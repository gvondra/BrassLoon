﻿using Autofac;
using BrassLoon.Client.Settings;

namespace BrassLoon.Client.DependencyInjection
{
    internal class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterInstance(AppSettingsLoader.Load());
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
        }
    }
}
