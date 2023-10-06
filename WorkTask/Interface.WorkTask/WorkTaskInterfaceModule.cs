using Autofac;

namespace BrassLoon.Interface.WorkTask
{
    public class WorkTaskInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<WorkGroupService>().As<IWorkGroupService>();
            builder.RegisterType<WorkTaskCommentService>().As<IWorkTaskCommentService>();
            builder.RegisterType<WorkTaskService>().As<IWorkTaskService>();
            builder.RegisterType<WorkTaskStatusService>().As<IWorkTaskStatusService>();
            builder.RegisterType<WorkTaskTypeService>().As<IWorkTaskTypeService>();
        }
    }
}
