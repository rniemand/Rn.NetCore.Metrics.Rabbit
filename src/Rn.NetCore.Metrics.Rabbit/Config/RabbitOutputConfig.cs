using Microsoft.Extensions.Configuration;

namespace Rn.NetCore.Metrics.Rabbit.Config;

// DOCS: docs\configuration\appsettings.md
public class RabbitOutputConfig
{
  public const string ConfigKey = "Rn.Metrics.Rabbit";

  [ConfigurationKeyName("enabled")]
  public bool Enabled { get; set; }

  [ConfigurationKeyName("username")]
  public string Username { get; set; } = "guest";

  [ConfigurationKeyName("password")]
  public string Password { get; set; } = "guest";

  [ConfigurationKeyName("virtualHost")]
  public string VirtualHost { get; set; } = "/";

  [ConfigurationKeyName("host")]
  public string Host { get; set; } = "127.0.0.1";

  [ConfigurationKeyName("port")]
  public int Port { get; set; } = 5672;

  [ConfigurationKeyName("exchange")]
  public string Exchange { get; set; } = "amq.topic";

  [ConfigurationKeyName("routingKey")]
  public string RoutingKey { get; set; } = "rn_core.metrics";

  [ConfigurationKeyName("backOffTimeSec")]
  public int BackOffTimeSec { get; set; } = 15;

  [ConfigurationKeyName("coolDownTimeSec")]
  public int CoolDownTimeSec { get; set; } = 300;

  [ConfigurationKeyName("coolDownThreshold")]
  public int CoolDownThreshold { get; set; } = 3;

  [ConfigurationKeyName("maxCoolDownRuns")]
  public int MaxCoolDownRuns { get; set; } = 0;
}
