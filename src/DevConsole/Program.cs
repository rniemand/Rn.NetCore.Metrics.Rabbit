using System;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Metrics;
using Rn.NetCore.Metrics.Builders;

namespace DevConsole;

class Program
{
  private static readonly IServiceProvider ServiceProvider = DIContainer.GetContainer();

  static void Main(string[] args)
  {
    var metrics = ServiceProvider.GetRequiredService<IMetricService>();

    var builder = new ServiceMetricBuilder(nameof(Program), nameof(Main));

    metrics.Submit(builder);
  }
}
