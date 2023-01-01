using Autofac;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Data
{
    public class LogDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance<ISqlDbProviderFactory>(new SqlClientProviderFactory());
            builder.RegisterType<EventIdDataFactory>().As<IEventIdDataFactory>();
            builder.RegisterType<EventIdDataSaver>().As<IEventIdDataSaver>();
            builder.RegisterType<ExceptionDataFactory>().As<IExceptionDataFactory>();
            builder.RegisterType<ExceptionDataSaver>().As<IExceptionDataSaver>();
            builder.RegisterType<MetricDataFactory>().As<IMetricDataFactory>();
            builder.RegisterType<MetricDataSaver>().As<IMetricDataSaver>();
            builder.RegisterType<PurgeDataSaver>().As<IPurgeDataSaver>();
            builder.RegisterType<PurgeWorkerDataFactory>().As<IPurgeWorkerDataFactory>();
            builder.RegisterType<PurgeWorkerDataSaver>().As<IPurgeWorkerDataSaver>();
            builder.RegisterType<TraceDataFactory>().As<ITraceDataFactory>();
            builder.RegisterType<TraceDataSaver>().As<ITraceDataSaver>();
        }
    }
}
