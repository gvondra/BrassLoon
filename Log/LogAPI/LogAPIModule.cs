﻿using Autofac;
namespace LogAPI
{
    public class LogAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MapperFactory>().SingleInstance();
            builder.RegisterType<SettingsFactory>().SingleInstance();
            builder.RegisterModule(new BrassLoon.Log.Core.LogModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
        }
    }
}
