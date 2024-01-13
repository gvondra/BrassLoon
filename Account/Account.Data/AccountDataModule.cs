using Autofac;
using BrassLoon.DataClient;

namespace BrassLoon.Account.Data
{
    public class AccountDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterInstance<ISqlDbProviderFactory>(new SqlClientProviderFactory());
            _ = builder.RegisterType<AccountDataFactory>().As<IAccountDataFactory>();
            _ = builder.RegisterType<AccountDataSaver>().As<IAccountDataSaver>();
            _ = builder.RegisterType<ClientCredentialDataFactory>().As<IClientCredentialDataFactory>();
            _ = builder.RegisterType<ClientCredentialDataSaver>().As<IClientCredentialDataSaver>();
            _ = builder.RegisterType<ClientDataFactory>().As<IClientDataFactory>();
            _ = builder.RegisterType<ClientDataSaver>().As<IClientDataSaver>();
            _ = builder.RegisterType<DomainDataFactory>().As<IDomainDataFactory>();
            _ = builder.RegisterType<DomainDataSaver>().As<IDomainDataSaver>();
            _ = builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            _ = builder.RegisterType<UserDataFactory>().As<IUserDataFactory>();
            _ = builder.RegisterType<UserDataSaver>().As<IUserDataSaver>();
            _ = builder.RegisterType<UserInvitationDataFactory>().As<IUserInvitationDataFactory>();
            _ = builder.RegisterType<UserInvitationDataSaver>().As<IUserInvitationDataSaver>();
        }
    }
}
