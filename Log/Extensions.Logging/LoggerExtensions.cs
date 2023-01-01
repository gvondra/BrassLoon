using BrassLoon.Interface.Log;
using BrassLoon.RestClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static ILogger LogMetric(this ILogger logger, Metric metric)
        {
            logger.Log(LogLevel.Information, 0, metric, null, formatter: LoggerExtensions.FormatMetric);
            return logger;
        }

        public static ILoggingBuilder AddBrassLoonLogger(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<RestUtil>();
            builder.Services.TryAddSingleton<IService, Service>();
            builder.Services.TryAddTransient<Account.ITokenService, Account.TokenService>();
            builder.Services.TryAddTransient<ITraceService, TraceService>();
            builder.Services.TryAddTransient<IExceptionService, ExceptionService>();
            builder.Services.TryAddTransient<IMetricService, MetricService>();
            builder.Services.TryAddSingleton<MessageFormatter>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());
            return builder;
        }

        public static ILoggingBuilder AddBrassLoonLogger(this ILoggingBuilder builder,
            Action<LoggerConfiguration> configure)
        {
            builder.AddBrassLoonLogger();
            builder.Services.Configure(configure);
            return builder;
        }

        public static string FormatMetric(Metric metric, System.Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder(metric.EventCode);
            if (metric.Magnitude.HasValue)
                stringBuilder.Append($" {Math.Round(metric.Magnitude.Value, 4)}");
            if (!string.IsNullOrEmpty(metric.Status))
                stringBuilder.Append($" (with status \"{metric.Status}\")");
            return stringBuilder.ToString();
        }
    }
}
