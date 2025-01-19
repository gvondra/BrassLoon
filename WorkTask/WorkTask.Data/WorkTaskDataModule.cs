using Autofac;
using BrassLoon.DataClient;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SqlClient = BrassLoon.WorkTask.Data.Internal.SqlClient;

namespace BrassLoon.WorkTask.Data
{
    public class WorkTaskDataModule : Module
    {
        private readonly bool _useMongoDb;

        public WorkTaskDataModule(bool useMongoDb = false)
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
            _ = builder.RegisterType<SqlClient.PurgeDataSaver>().As<IPurgeDataSaver>();
            _ = builder.RegisterType<SqlClient.PurgeWorkerDataFactory>().As<IPurgeWorkerDataFactory>();
            _ = builder.RegisterType<SqlClient.PurgeWorkerDataSaver>().As<IPurgeWorkerDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkGroupDataFactory>().As<IWorkGroupDataFactory>();
            _ = builder.RegisterType<SqlClient.WorkGroupDataSaver>().As<IWorkGroupDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkGroupMemberDataSaver>().As<IWorkGroupMemberDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkTaskCommentDataFactory>().As<IWorkTaskCommentDataFactory>();
            _ = builder.RegisterType<SqlClient.WorkTaskCommentDataSaver>().As<IWorkTaskCommentDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkTaskContextDataSaver>().As<IWorkTaskContextDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkTaskDataFactory>().As<IWorkTaskDataFactory>();
            _ = builder.RegisterType<SqlClient.WorkTaskDataSaver>().As<IWorkTaskDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkTaskStatusDataSaver>().As<IWorkTaskStatusDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkTaskTypeDataFactory>().As<IWorkTaskTypeDataFactory>();
            _ = builder.RegisterType<SqlClient.WorkTaskTypeDataSaver>().As<IWorkTaskTypeDataSaver>();
            _ = builder.RegisterType<SqlClient.WorkTaskTypeGroupDataSaver>().As<IWorkTaskTypeGroupDataSaver>();
        }

        private static void LoadMongoDb(ContainerBuilder builder)
        {
            _ = builder.RegisterType<DbProvider>().As<IDbProvider>();

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
