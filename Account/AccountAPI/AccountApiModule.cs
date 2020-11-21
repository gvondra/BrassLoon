using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class AccountApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Account.Core.AccountModule());
            builder.RegisterInstance<SettingsFactory>(new SettingsFactory());
        }
    }
}
