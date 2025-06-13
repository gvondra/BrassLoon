using Autofac;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.DataClient.SqlClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDb = BrassLoon.Log.Data.Internal.MongoDb;
using SqlClient = BrassLoon.Log.Data.Internal.SqlClient;

namespace BrassLoon.Log.Data
{
    public class LogDataModule : Module
    {
        private readonly bool _useMongoDb;

        public LogDataModule(bool useMongoDb = false)
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
            _ = builder.RegisterType<SqlClient.EventIdDataFactory>().As<IEventIdDataFactory>();
            _ = builder.RegisterType<SqlClient.EventIdDataSaver>().As<IEventIdDataSaver>();
            _ = builder.RegisterType<SqlClient.ExceptionDataFactory>().As<IExceptionDataFactory>();
            _ = builder.RegisterType<SqlClient.ExceptionDataSaver>().As<IExceptionDataSaver>();
            _ = builder.RegisterType<SqlClient.MetricDataFactory>().As<IMetricDataFactory>();
            _ = builder.RegisterType<SqlClient.MetricDataSaver>().As<IMetricDataSaver>();
            _ = builder.RegisterType<SqlClient.PurgeDataSaver>().As<IPurgeDataSaver>();
            _ = builder.RegisterType<SqlClient.PurgeWorkerDataFactory>().As<IPurgeWorkerDataFactory>();
            _ = builder.RegisterType<SqlClient.PurgeWorkerDataSaver>().As<IPurgeWorkerDataSaver>();
            _ = builder.RegisterType<SqlClient.TraceDataFactory>().As<ITraceDataFactory>();
            _ = builder.RegisterType<SqlClient.TraceDataSaver>().As<ITraceDataSaver>();
        }

        private static void LoadMongoDb(ContainerBuilder builder)
        {
            _ = builder.RegisterType<DbProvider>().As<IDbProvider>();
            _ = builder.RegisterType<MongoDb.EventIdDataFactory>().As<IEventIdDataFactory>();
            _ = builder.RegisterType<MongoDb.EventIdDataSaver>().As<IEventIdDataSaver>();
            _ = builder.RegisterType<MongoDb.ExceptionDataFactory>().As<IExceptionDataFactory>();
            _ = builder.RegisterType<MongoDb.ExceptionDataSaver>().As<IExceptionDataSaver>();
            _ = builder.RegisterType<MongoDb.MetricDataFactory>().As<IMetricDataFactory>();
            _ = builder.RegisterType<MongoDb.MetricDataSaver>().As<IMetricDataSaver>();
            _ = builder.RegisterType<MongoDb.TraceDataFactory>().As<ITraceDataFactory>();
            _ = builder.RegisterType<MongoDb.TraceDataSaver>().As<ITraceDataSaver>();
            // the following BsonClassMap are out of place. Just threw it here for simplicity
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            _ = BsonClassMap.RegisterClassMap<DataStateManager>();
            _ = BsonClassMap.RegisterClassMap<DataManagedStateBase>(cm =>
            {
                cm.AutoMap();
                _ = cm.MapProperty("Manager").SetShouldSerializeMethod(o => false);
            });
        }
    }
}
