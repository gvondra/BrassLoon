﻿using Autofac;
using BrassLoon.RestClient;

namespace BrassLoon.Interface.Account
{
    public class AccountInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<RestUtil>();
            builder.RegisterType<Service>().As<IService>();
            builder.RegisterType<DomainService>().As<IDomainService>();
            builder.RegisterType<TokenService>().As<ITokenService>();
        }
    }
}
