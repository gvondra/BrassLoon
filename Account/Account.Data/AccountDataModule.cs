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
            builder.RegisterInstance<ISqlDbProviderFactory>(new SqlClientProviderFactory());
            builder.RegisterType<AccountDataFactory>().As<IAccountDataFactory>();
            builder.RegisterType<AccountDataSaver>().As<IAccountDataSaver>();
            builder.RegisterType<ClientCredentialDataFactory>().As<IClientCredentialDataFactory>();
            builder.RegisterType<ClientCredentialDataSaver>().As<IClientCredentialDataSaver>();
            builder.RegisterType<ClientDataFactory>().As<IClientDataFactory>();
            builder.RegisterType<ClientDataSaver>().As<IClientDataSaver>();
            builder.RegisterType<DomainDataFactory>().As<IDomainDataFactory>();
            builder.RegisterType<DomainDataSaver>().As<IDomainDataSaver>();
            builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            builder.RegisterType<UserDataFactory>().As<IUserDataFactory>();
            builder.RegisterType<UserDataSaver>().As<IUserDataSaver>();
            builder.RegisterType<UserInvitationDataFactory>().As<IUserInvitationDataFactory>();
            builder.RegisterType<UserInvitationDataSaver>().As<IUserInvitationDataSaver>();
        }
    }
}
