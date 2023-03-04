using Autofac;
using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.WorkTask.Data.WorkTaskDataModule());
            builder.RegisterType<Saver>().SingleInstance();
            builder.RegisterType<WorkGroupFactory>().As<IWorkGroupFactory>();
            builder.RegisterType<WorkGroupSaver>().As<IWorkGroupSaver>();
            builder.RegisterType<WorkTaskFactory>().As<IWorkTaskFactory>();
            builder.RegisterType<WorkTaskSaver>().As<IWorkTaskSaver>();
            builder.RegisterType<WorkTaskStatusFactory>();
            builder.RegisterType<WorkTaskStatusFactory>().As<IWorkTaskStatusFactory>();
            builder.RegisterType<WorkTaskTypeFactory>();
            builder.RegisterType<WorkTaskTypeFactory>().As<IWorkTaskTypeFactory>();
            builder.RegisterType<WorkTaskTypeSaver>().As<IWorkTaskTypeSaver>();
        }
    }
}
