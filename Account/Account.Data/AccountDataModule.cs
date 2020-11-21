using Autofac;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Data
{
    public class AccountDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance<IDbProviderFactory>(new SqlClientProviderFactory());
            builder.RegisterType<AccountDataFactory>().As<IAccountDataFactory>();
            builder.RegisterType<AccountDataSaver>().As<IAccountDataSaver>();
            builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            builder.RegisterType<UserDataFactory>().As<IUserDataFactory>();
            builder.RegisterType<UserDataSaver>().As<IUserDataSaver>();
        }
    }
}
