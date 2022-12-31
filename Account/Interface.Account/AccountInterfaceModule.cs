using Autofac;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Text;

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
