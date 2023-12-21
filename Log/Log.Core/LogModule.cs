using Autofac;
using BrassLoon.Log.Framework;

namespace BrassLoon.Log.Core
{
    public class LogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Data.LogDataModule());
            _ = builder.RegisterInstance(new SettingsFactory());
            _ = builder.RegisterType<EventIdFactory>().As<IEventIdFactory>();
            _ = builder.RegisterType<ExceptionFactory>().As<IExceptionFactory>();
            _ = builder.RegisterType<ExceptionSaver>().As<IExceptionSaver>();
            _ = builder.RegisterType<MetricFactory>().As<IMetricFactory>();
            _ = builder.RegisterType<MetricSaver>().As<IMetricSaver>();
            _ = builder.RegisterType<PurgeSaver>().As<IPurgeSaver>();
            _ = builder.RegisterType<PurgeWorkerFactory>().As<IPurgeWorkerFactory>();
            _ = builder.RegisterType<PurgeWorkerSaver>().As<IPurgeWorkerSaver>();
            _ = builder.RegisterType<TraceFactory>().As<ITraceFactory>();
            _ = builder.RegisterType<TraceSaver>().As<ITraceSaver>();
        }
    }
}
