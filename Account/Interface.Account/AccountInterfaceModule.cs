using Autofac;
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
            builder.RegisterInstance(new RestUtil());
            builder.RegisterType<DomainService>().As<IDomainService>();
            builder.RegisterType<TokenService>().As<ITokenService>();
        }
    }
}
