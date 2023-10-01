using Autofac;
using BrassLoon.CommonAPI;
using WorkTaskRPC.Services;

namespace WorkTaskRPC
{
    public class WorkTakRPCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.WorkTask.Core.WorkTaskCoreModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterType<DomainAcountAccessVerifier>().As<IDomainAcountAccessVerifier>();
            builder.RegisterType<MetaDataProcessor>()
                .SingleInstance()
                .As<IMetaDataProcessor>();
            builder.RegisterType<SettingsFactory>().SingleInstance();
            builder.RegisterType<WorkGroupService>();
            builder.RegisterType<WorkTaskCommentService>();
            builder.RegisterType<WorkTaskService>();
            builder.RegisterType<WorkTaskStatusService>();
            builder.RegisterType<WorkTaskTypeService>();
        }
    }
}
