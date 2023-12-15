using Autofac;
using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Internal;

namespace BrassLoon.WorkTask.Data
{
    public class WorkTaskDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<BrassLoon.DataClient.SqlClientProviderFactory>().As<IDbProviderFactory>();
            _ = builder.RegisterType<BrassLoon.DataClient.LoaderFactory>().As<BrassLoon.DataClient.ILoaderFactory>();
            _ = builder.RegisterType<PurgeDataSaver>().As<IPurgeDataSaver>();
            _ = builder.RegisterType<PurgeWorkerDataFactory>().As<IPurgeWorkerDataFactory>();
            _ = builder.RegisterType<PurgeWorkerDataSaver>().As<IPurgeWorkerDataSaver>();
            _ = builder.RegisterType<WorkGroupDataFactory>().As<IWorkGroupDataFactory>();
            _ = builder.RegisterType<WorkGroupDataSaver>().As<IWorkGroupDataSaver>();
            _ = builder.RegisterType<WorkGroupMemberDataSaver>().As<IWorkGroupMemberDataSaver>();
            _ = builder.RegisterType<WorkTaskCommentDataFactory>().As<IWorkTaskCommentDataFactory>();
            _ = builder.RegisterType<WorkTaskCommentDataSaver>().As<IWorkTaskCommentDataSaver>();
            _ = builder.RegisterType<WorkTaskContextDataSaver>().As<IWorkTaskContextDataSaver>();
            _ = builder.RegisterType<WorkTaskDataFactory>().As<IWorkTaskDataFactory>();
            _ = builder.RegisterType<WorkTaskDataSaver>().As<IWorkTaskDataSaver>();
            _ = builder.RegisterType<WorkTaskStatusDataFactory>().As<IWorkTaskStatusDataFactory>();
            _ = builder.RegisterType<WorkTaskStatusDataSaver>().As<IWorkTaskStatusDataSaver>();
            _ = builder.RegisterType<WorkTaskTypeDataFactory>().As<IWorkTaskTypeDataFactory>();
            _ = builder.RegisterType<WorkTaskTypeDataSaver>().As<IWorkTaskTypeDataSaver>();
            _ = builder.RegisterType<WorkTaskTypeGroupDataSaver>().As<IWorkTaskTypeGroupDataSaver>();
        }
    }
}
