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
            builder.RegisterInstance<IDbProviderFactory>(new SqlClientProviderFactory());
            builder.RegisterType<ExceptionDataSaver>().As<IExceptionDataSaver>();
            builder.RegisterType<MetricDataSaver>().As<IMetricDataSaver>();
            builder.RegisterType<TraceDataSaver>().As<ITraceDataSaver>();
        }
    }
}
