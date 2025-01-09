using Autofac;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson.Serialization;
using MongoDb = BrassLoon.Address.Data.Internal.MongoDb;
using SqlClient = BrassLoon.Address.Data.Internal.SqlClient;

namespace BrassLoon.Address.Data
{
    public class AddressDataModule : Module
    {
        private readonly bool _useMongoDb;

        public AddressDataModule(bool useMongoDb = false)
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
            _ = builder.RegisterType<LoaderFactory>().As<ILoaderFactory>();
            _ = builder.RegisterType<SqlClient.AddressDataFactory>().As<IAddressDataFactory>();
            _ = builder.RegisterType<SqlClient.AddressDataSaver>().As<IAddressDataSaver>();
            _ = builder.RegisterType<SqlClient.EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<SqlClient.EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            _ = builder.RegisterType<SqlClient.PhoneDataFactory>().As<IPhoneDataFactory>();
            _ = builder.RegisterType<SqlClient.PhoneDataSaver>().As<IPhoneDataSaver>();
        }

        private static void LoadMongoDb(ContainerBuilder builder)
        {
            _ = builder.RegisterType<DbProvider>().As<IDbProvider>();
            _ = builder.RegisterType<MongoDb.AddressDataFactory>().As<IAddressDataFactory>();
            _ = builder.RegisterType<MongoDb.AddressDataSaver>().As<IAddressDataSaver>();
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
