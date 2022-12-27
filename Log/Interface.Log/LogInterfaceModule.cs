using Autofac;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Log
{
    public class LogInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<RestUtil>().SingleInstance();
            builder.RegisterType<Service>().As<IService>();
            builder.RegisterType<ExceptionService>().As<IExceptionService>();
            builder.RegisterType<MetricService>().As<IMetricService>();
            builder.RegisterType<TraceService>().As<ITraceService>();
        }
    }
}
