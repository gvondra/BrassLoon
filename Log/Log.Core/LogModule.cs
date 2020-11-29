using Autofac;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Core
{
    public class LogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Log.Data.LogDataModule());
            builder.RegisterType<TraceFactory>().As<ITraceFactory>();
            builder.RegisterType<TraceSaver>().As<ITraceSaver>();
        }
    }
}
