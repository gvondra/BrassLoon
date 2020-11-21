using Autofac;
using BrassLoon.Account.Data;
using BrassLoon.Account.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Core
{
    public class AccountModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new AccountDataModule());
            builder.RegisterInstance<SettingsFactory>(new SettingsFactory());
            builder.RegisterType<AccountFactory>().As<IAccountFactory>();
            builder.RegisterType<AccountSaver>().As<IAccountSaver>();
            builder.RegisterType<EmailAddressFactory>().As<IEmailAddressFactory>();
            builder.RegisterType<EmailAddressSaver>().As<IEmailAddressSaver>();
            builder.RegisterType<UserFactory>().As<IUserFactory>();
            builder.RegisterType<UserSaver>().As<IUserSaver>();
        }
    }
}
