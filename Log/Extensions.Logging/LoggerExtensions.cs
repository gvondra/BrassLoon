using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text;

namespace BrassLoon.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static ILogger LogMetric(this ILogger logger, Metric metric)
        {
            _ = logger.LogMetric(0, metric);
            return logger;
        }

        public static ILogger LogMetric(this ILogger logger, EventId eventId, Metric metric)
        {
            logger.Log(LogLevel.Information, eventId, metric, null, formatter: FormatMetric);
            return logger;
        }

        public static ILoggingBuilder AddBrassLoonLogger(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<MessageFormatter>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());
            return builder;
        }

        public static ILoggingBuilder AddBrassLoonLogger(this ILoggingBuilder builder,
            Action<LoggerConfiguration> configure)
        {
            _ = builder.AddBrassLoonLogger();
            _ = builder.Services.Configure(configure);
            return builder;
        }

        public static string FormatMetric(Metric metric, Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder(metric.EventCode);
            if (metric.Magnitude.HasValue)
#if NETSTANDARD
                _ = stringBuilder.Append($" {Math.Round(metric.Magnitude.Value, 4)}");
#else
                _ = stringBuilder.Append(CultureInfo.InvariantCulture, $" {Math.Round(metric.Magnitude.Value, 4)}");
#endif
            if (!string.IsNullOrEmpty(metric.Status))
#if NETSTANDARD
                _ = stringBuilder.Append($" (with status \"{metric.Status}\")");
#else
                _ = stringBuilder.Append(CultureInfo.InvariantCulture, $" (with status \"{metric.Status}\")");
#endif
            return stringBuilder.ToString();
        }
    }
}
