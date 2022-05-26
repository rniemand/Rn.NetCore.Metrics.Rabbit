using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Helpers;
using Rn.NetCore.Metrics.Extensions;
using Rn.NetCore.Metrics.Rabbit.Extensions;

namespace DevConsole;

internal class DIContainer
{
  public static IServiceProvider GetContainer()
  {
    var services = new ServiceCollection();

    var config = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .Build();

    services
      // Configuration
      .AddSingleton<IConfiguration>(config)

      // Abstractions
      .AddSingleton<IEnvironmentAbstraction, EnvironmentAbstraction>()
      .AddSingleton<IDirectoryAbstraction, DirectoryAbstraction>()
      .AddSingleton<IFileAbstraction, FileAbstraction>()

      // Helpers
      .AddSingleton<IJsonHelper, JsonHelper>()

      // Wrappers
      .AddSingleton<IPathAbstraction, PathAbstraction>()

      // Metrics
      .AddRnMetricsBase()
      .AddRnRabbitMQMetrics()

      // Logging
      .AddLogging(loggingBuilder =>
      {
        // configure Logging with NLog
        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
        loggingBuilder.AddNLog(config);
      });

    return services.BuildServiceProvider();
  }
}
