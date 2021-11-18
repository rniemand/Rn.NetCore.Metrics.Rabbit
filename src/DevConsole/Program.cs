﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Helpers;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Metrics;
using Rn.NetCore.Metrics.Builders;
using Rn.NetCore.Metrics.Outputs;
using Rn.NetCore.Metrics.Rabbit;
using Rn.NetCore.Metrics.Rabbit.Interfaces;

namespace DevConsole
{
  class Program
  {
    private static IServiceProvider _serviceProvider;
    private static ILoggerAdapter<Program> _logger;

    static void Main(string[] args)
    {
      ConfigureDI();

      var metrics = _serviceProvider.GetRequiredService<IMetricService>();

      var builder = new ServiceMetricBuilder(nameof(Program), nameof(Main));

      metrics.SubmitBuilder(builder);

      _logger.Info("All Done!");
    }


    // DI related methods
    private static void ConfigureDI()
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
        .AddSingleton<IDateTimeAbstraction, DateTimeAbstraction>()
        .AddSingleton<IEnvironmentAbstraction, EnvironmentAbstraction>()
        .AddSingleton<IDirectoryAbstraction, DirectoryAbstraction>()
        .AddSingleton<IFileAbstraction, FileAbstraction>()

        // Helpers
        .AddSingleton<IJsonHelper, JsonHelper>()

        // Wrappers
        .AddSingleton<IPathAbstraction, PathAbstraction>()

        // Metrics
        .AddSingleton<IMetricServiceUtils, MetricServiceUtils>()
        .AddSingleton<IMetricService, MetricService>()
        .AddSingleton<IRabbitFactory, RabbitFactory>()
        .AddSingleton<IRabbitConnection, RabbitConnection>()
        .AddSingleton<IMetricOutput, RabbitMetricOutput>()

        // Logging
        .AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>))
        .AddLogging(loggingBuilder =>
        {
          // configure Logging with NLog
          loggingBuilder.ClearProviders();
          loggingBuilder.SetMinimumLevel(LogLevel.Trace);
          loggingBuilder.AddNLog(config);
        });

      _serviceProvider = services.BuildServiceProvider();

      _serviceProvider = services.BuildServiceProvider();
      _logger = _serviceProvider.GetService<ILoggerAdapter<Program>>();
    }
  }
}
