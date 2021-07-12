using RabbitMQ.Client;
using Rn.NetCore.Metrics.Rabbit.Config;

namespace Rn.NetCore.Metrics.Rabbit
{
  public interface IRabbitFactory
  {
    IConnectionFactory CreateConnectionFactory(RabbitOutputConfig config);
  }

  public class RabbitFactory : IRabbitFactory
  {
    public IConnectionFactory CreateConnectionFactory(RabbitOutputConfig config)
    {
      // TODO: [TESTS] (RabbitFactory.CreateConnectionFactory) Add tests
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
}
