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
            builder.RegisterType<BrassLoon.DataClient.SqlClientProviderFactory>().As<IDbProviderFactory>();
            builder.RegisterType<WorkGroupDataFactory>().As<IWorkGroupDataFactory>();
            builder.RegisterType<WorkGroupDataSaver>().As<IWorkGroupDataSaver>();
            builder.RegisterType<WorkGroupMemberDataSaver>().As<IWorkGroupMemberDataSaver>();
            builder.RegisterType<WorkTaskCommentDataFactory>().As<IWorkTaskCommentDataFactory>();
            builder.RegisterType<WorkTaskCommentDataSaver>().As<IWorkTaskCommentDataSaver>();
            builder.RegisterType<WorkTaskContextDataSaver>().As<IWorkTaskContextDataSaver>();
            builder.RegisterType<WorkTaskDataFactory>().As<IWorkTaskDataFactory>();
            builder.RegisterType<WorkTaskDataSaver>().As<IWorkTaskDataSaver>();
            builder.RegisterType<WorkTaskStatusDataFactory>().As<IWorkTaskStatusDataFactory>();
            builder.RegisterType<WorkTaskStatusDataSaver>().As<IWorkTaskStatusDataSaver>();
            builder.RegisterType<WorkTaskTypeDataFactory>().As<IWorkTaskTypeDataFactory>();
            builder.RegisterType<WorkTaskTypeDataSaver>().As<IWorkTaskTypeDataSaver>();
            builder.RegisterType<WorkTaskTypeGroupDataSaver>().As<IWorkTaskTypeGroupDataSaver>();
        }
    }
}
