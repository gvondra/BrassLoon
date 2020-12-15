using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI
{
    public class LogAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(new SettingsFactory());
            builder.RegisterModule(new BrassLoon.Log.Core.LogModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
        }
    }

    public static class LogAPIModuleExtensions
    {
        public static IServiceCollection AddDiContainer(this IServiceCollection services)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new LogAPIModule());
            services.AddSingleton<IContainer>(builder.Build());
            return services;
        }
    }
}
