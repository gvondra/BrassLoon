using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ConfigAPI
{
    public class ConfigApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(new SettingsFactory());
            builder.RegisterModule(new BrassLoon.Config.Core.ConfigModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
        }
    }

    public static class ConfigAPIModuleExtensions
    {
        public static IServiceCollection AddDiContainer(this IServiceCollection services)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigApiModule());
            services.AddSingleton<IContainer>(builder.Build());
            return services;
        }
    }
}
