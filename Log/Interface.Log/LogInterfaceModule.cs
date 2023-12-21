using Autofac;
using BrassLoon.RestClient;

namespace BrassLoon.Interface.Log
{
    public class LogInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<RestUtil>().SingleInstance();
            _ = builder.RegisterType<Service>().As<IService>();
            _ = builder.RegisterType<ExceptionService>().As<IExceptionService>();
            _ = builder.RegisterType<MetricService>().As<IMetricService>();
            _ = builder.RegisterType<TraceService>().As<ITraceService>();
        }
    }
}
