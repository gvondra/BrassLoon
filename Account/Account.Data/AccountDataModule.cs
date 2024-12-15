using Autofac;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

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
            _ = builder.RegisterType<Internal.SqlClient.AccountDataFactory>().As<IAccountDataFactory>();
            _ = builder.RegisterType<Internal.SqlClient.AccountDataSaver>().As<IAccountDataSaver>();
            _ = builder.RegisterType<Internal.SqlClient.ClientCredentialDataFactory>().As<IClientCredentialDataFactory>();
            _ = builder.RegisterType<Internal.SqlClient.ClientCredentialDataSaver>().As<IClientCredentialDataSaver>();
            _ = builder.RegisterType<Internal.SqlClient.ClientDataFactory>().As<IClientDataFactory>();
            _ = builder.RegisterType<Internal.SqlClient.ClientDataSaver>().As<IClientDataSaver>();
            _ = builder.RegisterType<Internal.SqlClient.DomainDataFactory>().As<IDomainDataFactory>();
            _ = builder.RegisterType<Internal.SqlClient.DomainDataSaver>().As<IDomainDataSaver>();
            _ = builder.RegisterType<Internal.SqlClient.EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<Internal.SqlClient.EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            _ = builder.RegisterType<Internal.SqlClient.UserDataFactory>().As<IUserDataFactory>();
            _ = builder.RegisterType<Internal.SqlClient.UserDataSaver>().As<IUserDataSaver>();
            _ = builder.RegisterType<Internal.SqlClient.UserInvitationDataFactory>().As<IUserInvitationDataFactory>();
            _ = builder.RegisterType<Internal.SqlClient.UserInvitationDataSaver>().As<IUserInvitationDataSaver>();
        }

        private static void LoadMongoDb(ContainerBuilder builder)
        {
            _ = builder.RegisterType<DbProvider>().As<IDbProvider>();
            _ = builder.RegisterType<Internal.MongoDb.AccountDataFactory>().As<IAccountDataFactory>();
            _ = builder.RegisterType<Internal.MongoDb.AccountDataSaver>().As<IAccountDataSaver>();
            _ = builder.RegisterType<Internal.MongoDb.ClientCredentialDataFactory>().As<IClientCredentialDataFactory>();
            _ = builder.RegisterType<Internal.MongoDb.ClientCredentialDataSaver>().As<IClientCredentialDataSaver>();
            _ = builder.RegisterType<Internal.MongoDb.ClientDataFactory>().As<IClientDataFactory>();
            _ = builder.RegisterType<Internal.MongoDb.ClientDataSaver>().As<IClientDataSaver>();
            _ = builder.RegisterType<Internal.MongoDb.DomainDataFactory>().As<IDomainDataFactory>();
            _ = builder.RegisterType<Internal.MongoDb.DomainDataSaver>().As<IDomainDataSaver>();
            _ = builder.RegisterType<Internal.MongoDb.EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<Internal.MongoDb.EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            _ = builder.RegisterType<Internal.MongoDb.UserDataFactory>().As<IUserDataFactory>();
            _ = builder.RegisterType<Internal.MongoDb.UserDataSaver>().As<IUserDataSaver>();
            _ = builder.RegisterType<Internal.MongoDb.UserInvitationDataFactory>().As<IUserInvitationDataFactory>();
            _ = builder.RegisterType<Internal.MongoDb.UserInvitationDataSaver>().As<IUserInvitationDataSaver>();
            // the following BsonClassMap are out of place. Just threw it here for simplicity
            _ = BsonClassMap.RegisterClassMap<DataStateManager>();
            _ = BsonClassMap.RegisterClassMap<DataManagedStateBase>(cm =>
            {
                cm.AutoMap();
                _ = cm.MapProperty("Manager").SetShouldSerializeMethod(o => false);
            });
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
    }
}
