using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics.Interfaces;
using Rn.NetCore.Common.Metrics.Models;
using Rn.NetCore.Metrics.Rabbit.Config;

namespace Rn.NetCore.Metrics.Rabbit
{
  public class RabbitMetricOutput : IMetricOutput
  {
    public bool Enabled { get; }
    public string Name { get; }

    private readonly ILoggerAdapter<RabbitMetricOutput> _logger;
    private readonly IRabbitConnection _connection;
    private readonly RabbitOutputConfig _config;

    public const string ConfigKey = "RnCore:Metrics:RabbitOutput";

    public RabbitMetricOutput(
      ILoggerAdapter<RabbitMetricOutput> logger,
      IRabbitConnection connection,
      IConfiguration configuration)
    {
      // TODO: [TESTS] (RabbitMetricOutput) Add tests
      _logger = logger;
      _connection = connection;
      _config = BindConfiguration(configuration);

      Enabled = _config.Enabled;
      Name = nameof(RabbitMetricOutput);

      if (!Enabled)
        return;

      _connection.Configure(_config);
    }

    // Interface methods
    public async Task SubmitPoint(LineProtocolPoint point)
    {
      // TODO: [TESTS] (RabbitMetricOutput.SubmitPoint) Add tests
      if(!Enabled)
        return;

      await _connection.SubmitPoint(point);
    }

    public async Task SubmitPoints(List<LineProtocolPoint> points)
    {
      // TODO: [TESTS] (RabbitMetricOutput.SubmitPoints) Add tests
      if(!Enabled)
        return;

      await _connection.SubmitPoints(points);
    }

    // Configuration related methods
    private RabbitOutputConfig BindConfiguration(IConfiguration configuration)
    {
      // TODO: [TESTS] (RabbitMetricOutput.BindConfiguration) Add tests
      var boundConfiguration = new RabbitOutputConfig();
      var configSection = configuration.GetSection(ConfigKey);

      if (!configSection.Exists())
      {
        _logger.Warning(
          "Unable to find configuration section '{key}', using defaults",
          ConfigKey
        );

        return boundConfiguration;
      }

      configSection.Bind(boundConfiguration);
      return boundConfiguration;
    }
  }
}
