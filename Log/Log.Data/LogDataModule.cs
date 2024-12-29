using Autofac;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Internal.SqlClient;
using MongoDB.Bson.Serialization;

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
            _ = builder.RegisterType<EventIdDataFactory>().As<IEventIdDataFactory>();
            _ = builder.RegisterType<EventIdDataSaver>().As<IEventIdDataSaver>();
            _ = builder.RegisterType<ExceptionDataFactory>().As<IExceptionDataFactory>();
            _ = builder.RegisterType<ExceptionDataSaver>().As<IExceptionDataSaver>();
            _ = builder.RegisterType<MetricDataFactory>().As<IMetricDataFactory>();
            _ = builder.RegisterType<MetricDataSaver>().As<IMetricDataSaver>();
            _ = builder.RegisterType<PurgeDataSaver>().As<IPurgeDataSaver>();
            _ = builder.RegisterType<PurgeWorkerDataFactory>().As<IPurgeWorkerDataFactory>();
            _ = builder.RegisterType<PurgeWorkerDataSaver>().As<IPurgeWorkerDataSaver>();
            _ = builder.RegisterType<TraceDataFactory>().As<ITraceDataFactory>();
            _ = builder.RegisterType<TraceDataSaver>().As<ITraceDataSaver>();
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
