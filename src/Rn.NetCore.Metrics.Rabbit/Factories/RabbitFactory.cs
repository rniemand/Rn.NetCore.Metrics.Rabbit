using RabbitMQ.Client;
using Rn.NetCore.Metrics.Rabbit.Config;

namespace Rn.NetCore.Metrics.Rabbit.Factories;

// DOCS: docs\factories\RabbitFactory.md
public class RabbitFactory : IRabbitFactory
{
  public IConnectionFactory CreateConnectionFactory(RabbitOutputConfig config)
  {
    return new ConnectionFactory
    {
      UserName = config.Username,
      Password = config.Password,
      VirtualHost = config.VirtualHost,
      HostName = config.Host,
      Port = config.Port
    };
  }
}
