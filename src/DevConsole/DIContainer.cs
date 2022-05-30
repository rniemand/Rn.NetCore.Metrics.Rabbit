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
      .AddSingleton<IConfiguration>(config)
      .AddSingleton<IEnvironmentAbstraction, EnvironmentAbstraction>()
      .AddSingleton<IDirectoryAbstraction, DirectoryAbstraction>()
      .AddSingleton<IFileAbstraction, FileAbstraction>()
      .AddSingleton<IJsonHelper, JsonHelper>()
      .AddSingleton<IPathAbstraction, PathAbstraction>()
      .AddSingleton<IDateTimeAbstraction, DateTimeAbstraction>()

      // Metrics
      .AddRnMetricsBase(config)
      .AddRnMetricRabbitMQ()

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
