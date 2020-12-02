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
            builder.RegisterType<MetricFactory>().As<IMetricFactory>();
            builder.RegisterType<MetricSaver>().As<IMetricSaver>();
            builder.RegisterType<TraceFactory>().As<ITraceFactory>();
            builder.RegisterType<TraceSaver>().As<ITraceSaver>();
        }
    }
}
