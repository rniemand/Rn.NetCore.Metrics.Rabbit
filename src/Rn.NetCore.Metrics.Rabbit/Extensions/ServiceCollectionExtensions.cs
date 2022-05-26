using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rn.NetCore.Metrics.Outputs;
using Rn.NetCore.Metrics.Rabbit.Interfaces;

namespace Rn.NetCore.Metrics.Rabbit.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddRnRabbitMQMetrics(this IServiceCollection services)
  {
    services.TryAddSingleton<IRabbitFactory, RabbitFactory>();
    services.TryAddSingleton<IRabbitConnection, RabbitConnection>();
    services.TryAddSingleton<IMetricOutput, RabbitMetricOutput>();
    
    return services;
  }
}
