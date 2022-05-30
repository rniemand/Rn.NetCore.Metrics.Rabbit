using RabbitMQ.Client;
using Rn.NetCore.Metrics.Rabbit.Config;

namespace Rn.NetCore.Metrics.Rabbit.Factories;

public interface IRabbitFactory
{
  IConnectionFactory CreateConnectionFactory(RabbitOutputConfig config);
}