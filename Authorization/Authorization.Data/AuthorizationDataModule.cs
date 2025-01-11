using Autofac;
using BrassLoon.Authorization.Data.Framework;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson.Serialization;
//using InternalMongoDb = BrassLoon.Authorization.Data.Internal.MongoDb;
using InternalSqlClient = BrassLoon.Authorization.Data.Internal.SqlClient;

namespace BrassLoon.Authorization.Data
{
    public class AuthorizationDataModule : Module
    {
        private readonly bool _useMongoDb;

        public AuthorizationDataModule(bool useMongoDb = false)
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
            _ = builder.RegisterType<InternalSqlClient.ClientDataFactory>().As<IClientDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.ClientDataSaver>().As<IClientDataSaver>();
            _ = builder.RegisterType<InternalSqlClient.EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            _ = builder.RegisterType<InternalSqlClient.RoleDataFactory>().As<IRoleDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.RoleDataSaver>().As<IRoleDataSaver>();
            _ = builder.RegisterType<InternalSqlClient.SigningKeyDataFactory>().As<ISigningKeyDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.SigningKeyDataSaver>().As<ISigningKeyDataSaver>();
            _ = builder.RegisterType<InternalSqlClient.UserDataFactory>().As<IUserDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.UserDataSaver>().As<IUserDataSaver>();
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
