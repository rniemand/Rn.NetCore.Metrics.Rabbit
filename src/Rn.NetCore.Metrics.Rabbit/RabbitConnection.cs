using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Metrics.Rabbit.Config;
using Rn.NetCore.Metrics.Rabbit.Factories;
using Rn.NetCore.Metrics.Rabbit.Models;

namespace Rn.NetCore.Metrics.Rabbit;

// DOCS: docs\RabbitConnection.md
public class RabbitConnection : IRabbitConnection
{
  private readonly ILoggerAdapter<RabbitConnection> _logger;
  private readonly IRabbitFactory _rabbitFactory;
  private readonly IDateTimeAbstraction _dateTime;

  private RabbitOutputConfig _config;
  private IConnectionFactory _connectionFactory;
  private IConnection _connection;
  private bool _connectionEnabled;
  private IModel _channel;

  private DateTime? _disabledUntil;
  private int _connectionErrorCount;
  private int _coolDownRunCount;

  public RabbitConnection(
    ILoggerAdapter<RabbitConnection> logger,
    IRabbitFactory rabbitFactory,
    IDateTimeAbstraction dateTime)
  {
    _logger = logger;
    _rabbitFactory = rabbitFactory;
    _dateTime = dateTime;
    _config = new RabbitOutputConfig();

    _connectionEnabled = _config.Enabled;
    _disabledUntil = null;
    _connectionErrorCount = 0;
    _coolDownRunCount = 0;
  }


  // Interface methods
  public void Configure(RabbitOutputConfig config)
  {
    _config = config;
    _connectionEnabled = config.Enabled;

    // Ensure that we are enabled
    if (!_config.Enabled)
      return;

    CreateConnectionFactory();
    RecreateConnection();
  }

  public async Task SubmitPoint(LineProtocolPoint point)
  {
    await SubmitPoints(new List<LineProtocolPoint> { point });
  }

  public async Task SubmitPoints(List<LineProtocolPoint> points)
  {
    if (!CanSubmitPoints())
      return;

    PublishPoints(points);
    await Task.CompletedTask;
  }


  // Connection related methods
  private void TearDownConnection()
  {
    if (_channel == null && _connection == null)
      return;

    _logger.LogTrace("Tearing down RabbitMQ connection");

    try
    {
      _channel?.Close();
      _connection?.Close();

      _channel?.Dispose();
      _connection?.Dispose();

      _channel = null;
      _connection = null;
    }
    catch (Exception ex)
    {
      _logger.LogUnexpectedException(ex);
    }
  }

  private void CreateConnectionFactory()
  {
    _logger.LogTrace("Creating new connection factory");
    _connectionFactory = _rabbitFactory.CreateConnectionFactory(_config);
  }

  private void RecreateConnection()
  {
    TearDownConnection();

    try
    {
      _connection = _connectionFactory.CreateConnection();
      _channel = _connection.CreateModel();
      HandleConnectionSuccess();
    }
    catch (Exception ex)
    {
      _logger.LogUnexpectedException(ex);
      HandleConnectionError();
    }
  }

  private bool CurrentlyConnected()
  {
    if (!(_connection?.IsOpen ?? false))
      return false;

    return _channel?.IsOpen ?? false;
  }


  // Point submitting related methods
  private bool CanSubmitPoints()
  {
    // Check to see if we have been disabled via "HandleMaxCoolDownRuns()"
    if (!_connectionEnabled)
      return false;

    // Check to see if we have been globally disabled somehow
    if (!_config.Enabled)
      return false;

    // Check to see if we are currently in a "cooldown" or "back-off" loop
    if (_disabledUntil.HasValue)
    {
      if (_dateTime.Now < _disabledUntil.Value)
        return false;

      _disabledUntil = null;
    }

    // Check the current connection state (attempt a reconnection if required)
    if (CurrentlyConnected())
      return true;

    RecreateConnection();
    return CurrentlyConnected();
  }

  private static string GeneratePayload(IEnumerable<LineProtocolPoint> points)
  {
    using var sw = new StringWriter();
    foreach (var point in points)
    {
      point.Format(sw);
      sw.Write("\n");
    }

    return sw.ToString().Trim();
  }

  private void PublishPoints(IReadOnlyCollection<LineProtocolPoint> points)
  {
    if (points.Count == 0)
      return;

    try
    {
      _channel?.BasicPublish(
        exchange: _config.Exchange,
        routingKey: _config.RoutingKey,
        basicProperties: null,
        body: Encoding.UTF8.GetBytes(GeneratePayload(points))
      );

      HandleConnectionSuccess();
    }
    catch (Exception ex)
    {
      _logger.LogUnexpectedException(ex);
      HandleConnectionError();
    }
  }


  // Backing-off related methods
  private void HandleConnectionSuccess()
  {
    _disabledUntil = null;
    _connectionErrorCount = 0;
    _coolDownRunCount = 0;
  }

  private void HandleConnectionError()
  {
    _connectionErrorCount += 1;

    // Enter cooldown if we hit the configured threshold
    if (_connectionErrorCount >= _config.CoolDownThreshold)
    {
      HandleCoolDown();
      return;
    }

    // Temporarily disable the connection for a few seconds
    _disabledUntil = _dateTime.Now.AddSeconds(_config.BackOffTimeSec);

    _logger.LogInformation(
      "Backing off Rabbit connection for {s} second(s), will try again at {d}",
      _config.BackOffTimeSec,
      _disabledUntil
    );
  }

  private void HandleCoolDown()
  {
    _connectionErrorCount = 0;
    _coolDownRunCount += 1;

    // If configured, and hit - disable the connection after "x" cooldown runs
    if (_config.MaxCoolDownRuns > 0 && _coolDownRunCount > _config.MaxCoolDownRuns)
    {
      HandleMaxCoolDownRuns();
      return;
    }

    // Enter into a cooldown in hopes that the RabbitMQ connection will come back
    _disabledUntil = _dateTime.Now.AddSeconds(_config.CoolDownTimeSec);

    _logger.LogWarning(
      "Failed to communicate with RabbitMQ {x} time(s) in a row - backing off until {d}",
      _config.CoolDownThreshold,
      _disabledUntil
    );
  }

  private void HandleMaxCoolDownRuns()
  {
    _logger.LogError("It seems that we are unable to connect to RabbitMQ, disabling output");

    _connectionEnabled = false;
    _connectionErrorCount = 0;
    _coolDownRunCount = 0;
    _disabledUntil = null;
  }
}
