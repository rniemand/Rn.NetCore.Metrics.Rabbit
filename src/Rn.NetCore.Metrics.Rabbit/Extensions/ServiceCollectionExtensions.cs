using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Metrics.Outputs;
using Rn.NetCore.Metrics.Rabbit.Factories;

namespace Rn.NetCore.Metrics.Rabbit.Extensions;

// DOCS: docs\extensions\ServiceCollectionExtensions.md
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddRnMetricRabbitMQ(this IServiceCollection services)
  {
    services.TryAddSingleton<IDateTimeAbstraction, DateTimeAbstraction>();
    services.TryAddSingleton<IRabbitFactory, RabbitFactory>();
    services.TryAddSingleton<IRabbitConnection, RabbitConnection>();
    services.AddSingleton<IMetricOutput, RabbitMetricOutput>();
    
    return services;
  }
}
