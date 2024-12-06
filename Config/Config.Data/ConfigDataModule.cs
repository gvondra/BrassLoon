using Autofac;
using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson.Serialization;
using InternalMongoDb = BrassLoon.Config.Data.Internal.MongoDb;
using InternalSqlClient = BrassLoon.Config.Data.Internal.SqlClient;

namespace BrassLoon.Config.Data
{
    public class ConfigDataModule : Module
    {
        private readonly bool _useMongoDb;

        public ConfigDataModule(bool useMongoDb = false)
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
            _ = builder.RegisterType<InternalSqlClient.ItemDataFactory>().As<IItemDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.ItemDataSaver>().As<IItemDataSaver>();
            _ = builder.RegisterType<InternalSqlClient.ItemHistoryDataFactory>().As<IItemHistoryDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.LookupDataFactory>().As<ILookupDataFactory>();
            _ = builder.RegisterType<InternalSqlClient.LookupDataSaver>().As<ILookupDataSaver>();
            _ = builder.RegisterType<InternalSqlClient.LookupHistoryDataFactory>().As<ILookupHistoryDataFactory>();
        }

        private static void LoadMongoDb(ContainerBuilder builder)
        {
            _ = builder.RegisterType<DbProvider>().As<IDbProvider>();
            _ = builder.RegisterType<InternalMongoDb.ItemDataFactory>().As<IItemDataFactory>();
            _ = builder.RegisterType<InternalMongoDb.ItemDataSaver>().As<IItemDataSaver>();
            _ = builder.RegisterType<InternalMongoDb.ItemHistoryDataFactory>().As<IItemHistoryDataFactory>();
            _ = builder.RegisterType<InternalMongoDb.LookupDataFactory>().As<ILookupDataFactory>();
            _ = builder.RegisterType<InternalMongoDb.LookupDataSaver>().As<ILookupDataSaver>();
            _ = builder.RegisterType<InternalMongoDb.LookupHistoryDataFactory>().As<ILookupHistoryDataFactory>();

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
