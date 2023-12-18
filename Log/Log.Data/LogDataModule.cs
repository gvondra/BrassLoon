using Autofac;
using BrassLoon.DataClient;

namespace BrassLoon.Log.Data
{
    public class LogDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterInstance<ISqlDbProviderFactory>(new SqlClientProviderFactory());
            _ = builder.RegisterType<EventIdDataFactory>().As<IEventIdDataFactory>();
            _ = builder.RegisterType<EventIdDataSaver>().As<IEventIdDataSaver>();
            _ = builder.RegisterType<ExceptionDataFactory>().As<IExceptionDataFactory>();
            _ = builder.RegisterType<ExceptionDataSaver>().As<IExceptionDataSaver>();
            _ = builder.RegisterType<MetricDataFactory>().As<IMetricDataFactory>();
            _ = builder.RegisterType<MetricDataSaver>().As<IMetricDataSaver>();
            _ = builder.RegisterType<PurgeDataSaver>().As<IPurgeDataSaver>();
            _ = builder.RegisterType<PurgeWorkerDataFactory>().As<IPurgeWorkerDataFactory>();
            _ = builder.RegisterType<PurgeWorkerDataSaver>().As<IPurgeWorkerDataSaver>();
            _ = builder.RegisterType<TraceDataFactory>().As<ITraceDataFactory>();
            _ = builder.RegisterType<TraceDataSaver>().As<ITraceDataSaver>();
        }
    }
}
