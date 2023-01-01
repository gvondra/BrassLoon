using Autofac;
using BrassLoon.Log.Framework;

namespace BrassLoon.Log.Core
{
    public class LogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Log.Data.LogDataModule());
            builder.RegisterInstance(new SettingsFactory());
            builder.RegisterType<EventIdFactory>().As<IEventIdFactory>();
            builder.RegisterType<ExceptionFactory>().As<IExceptionFactory>();
            builder.RegisterType<ExceptionSaver>().As<IExceptionSaver>();
            builder.RegisterType<MetricFactory>().As<IMetricFactory>();
            builder.RegisterType<MetricSaver>().As<IMetricSaver>();
            builder.RegisterType<PurgeSaver>().As<IPurgeSaver>();
            builder.RegisterType<PurgeWorkerFactory>().As<IPurgeWorkerFactory>();
            builder.RegisterType<PurgeWorkerSaver>().As<IPurgeWorkerSaver>();
            builder.RegisterType<TraceFactory>().As<ITraceFactory>();
            builder.RegisterType<TraceSaver>().As<ITraceSaver>();
        }
    }
}
