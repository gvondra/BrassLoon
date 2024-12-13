using Autofac;
using BrassLoon.Account.Data.Internal.SqlClient;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson.Serialization;

namespace BrassLoon.Account.Data
{
    public class AccountDataModule : Module
    {
        private readonly bool _useMongoDb;

        public AccountDataModule(bool useMongoDb = false)
        {
            _useMongoDb = useMongoDb;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            if (_useMongoDb)
                LoadMongoDb(builder);
            else
                LoadSqlClient(builder);
        }

        private static void LoadSqlClient(ContainerBuilder builder)
        {
            _ = builder.RegisterGeneric(typeof(GenericDataFactory<>))
                .InstancePerLifetimeScope()
                .As(typeof(IGenericDataFactory<>));
            _ = builder.RegisterType<SqlClientProviderFactory>()
                .As<IDbProviderFactory>()
                .As<ISqlDbProviderFactory>();
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

        private static void LoadMongoDb(ContainerBuilder builder)
        {
            _ = builder.RegisterType<DbProvider>().As<IDbProvider>();
            // the following BsonClassMap are out of place. Just threw it here for simplicity
            _ = BsonClassMap.RegisterClassMap<DataStateManager>();
            _ = BsonClassMap.RegisterClassMap<DataManagedStateBase>(cm =>
            {
                cm.AutoMap();
                _ = cm.MapProperty("Manager").SetShouldSerializeMethod(o => false);
            });
        }
    }
}
