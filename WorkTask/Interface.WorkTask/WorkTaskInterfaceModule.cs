using Autofac;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<WorkGroupService>().As<IWorkGroupService>();
            _ = builder.RegisterType<WorkTaskCommentService>().As<IWorkTaskCommentService>();
            _ = builder.RegisterType<WorkTaskService>().As<IWorkTaskService>();
            _ = builder.RegisterType<WorkTaskStatusService>().As<IWorkTaskStatusService>();
            _ = builder.RegisterType<WorkTaskTypeService>().As<IWorkTaskTypeService>();
        }
    }
}
