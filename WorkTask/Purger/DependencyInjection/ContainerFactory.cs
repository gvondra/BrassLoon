using Autofac;
using BrassLoon.Extensions.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace BrassLoon.WorkTask.Purger.DependencyInjection
{
    public static class ContainerFactory
    {
        private static IContainer _container;

        static ContainerFactory()
        {
            Initialize();
        }

        public static void Initialize(
            AppSettings appSettings = null)
        {
            ContainerBuilder builder = new ContainerBuilder();
            _ = builder.RegisterModule(new ContainerModule());
            if (appSettings != null)
            {
                _ = builder.RegisterInstance(appSettings);
                if (!string.IsNullOrEmpty(appSettings.BrassLoonLogRpcBaseAddress) && appSettings.LoggingClientId.HasValue)
                {
                    RegisterLogging(builder, appSettings);
                }
            }
            _container = builder.Build();
        }

        private static void RegisterLogging(ContainerBuilder builder, AppSettings appSettings)
        {
            _ = builder.Register(c => LoggerFactory.Create(b =>
            {
                _ = b.AddBrassLoonLogger(config =>
                {
                    config.LogApiBaseAddress = appSettings.BrassLoonLogRpcBaseAddress;
                    config.LogDomainId = appSettings.LoggingDomainId.Value;
                    config.LogClientId = appSettings.LoggingClientId.Value;
                    config.LogClientSecret = appSettings.LoggingClientSecret;
                })
                .AddConsole();
            })).SingleInstance();
            _ = builder.RegisterGeneric((context, types) =>
            {
                ILoggerFactory loggerFactory = context.Resolve<ILoggerFactory>();
                Type factoryType = typeof(LoggerFactoryExtensions);
                MethodInfo methodInfo = factoryType.GetMethod("CreateLogger", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(ILoggerFactory) });
                methodInfo = methodInfo.MakeGenericMethod(types);
                return methodInfo.Invoke(null, new object[] { loggerFactory });

            }).As(typeof(ILogger<>));
            _ = builder.Register<string, ILogger>((context, categoryName) =>
            {
                ILoggerFactory loggerFactory = context.Resolve<ILoggerFactory>();
                return loggerFactory.CreateLogger(categoryName);
            });
        }

        public static ILifetimeScope BeginLifetimeScope() => _container.BeginLifetimeScope();
    }
}
