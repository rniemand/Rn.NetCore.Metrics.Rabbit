using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Metrics.Models;
using Rn.NetCore.Metrics.Outputs;
using Rn.NetCore.Metrics.Rabbit.Config;
using Rn.NetCore.Metrics.Rabbit.Interfaces;
using Rn.NetCore.Metrics.Rabbit.Models;

namespace Rn.NetCore.Metrics.Rabbit;

public class RabbitMetricOutput : IMetricOutput
{
  public bool Enabled { get; }
  public string Name { get; }

  private readonly ILoggerAdapter<RabbitMetricOutput> _logger;
  private readonly IRabbitConnection _connection;

  public const string ConfigKey = "Rn.Metrics.Rabbit";

  public RabbitMetricOutput(
    ILoggerAdapter<RabbitMetricOutput> logger,
    IRabbitConnection connection,
    IConfiguration configuration)
  {
    // TODO: [TESTS] (RabbitMetricOutput) Add tests
    _logger = logger;
    _connection = connection;
    var config = BindConfiguration(configuration);

    Enabled = config.Enabled;
    Name = nameof(RabbitMetricOutput);

    if (!Enabled)
      return;

    _connection.Configure(config);
  }


  // Interface methods
  public async Task SubmitMetric(CoreMetric metric)
    => await SubmitMetrics(new List<CoreMetric> { metric });

  public async Task SubmitMetrics(List<CoreMetric> metrics)
  {
    // TODO: [TESTS] (RabbitMetricOutput.SubmitMetrics) Add tests
    var points = metrics.Select(metric =>
      new LineProtocolPoint(
        metric.Measurement,
        metric.Fields,
        metric.Tags,
        metric.UtcTimestamp
      )).ToList();

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
      _logger.LogWarning(
        "Unable to find configuration section '{key}', using defaults",
        ConfigKey
      );

      return boundConfiguration;
    }

    configSection.Bind(boundConfiguration);
    return boundConfiguration;
  }
}
