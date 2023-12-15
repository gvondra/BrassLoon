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
            _ = builder.RegisterModule(new BrassLoon.WorkTask.Core.WorkTaskCoreModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterType<DomainAcountAccessVerifier>().As<IDomainAcountAccessVerifier>();
            _ = builder.RegisterType<MetaDataProcessor>()
                .SingleInstance()
                .As<IMetaDataProcessor>();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
            _ = builder.RegisterType<WorkGroupService>();
            _ = builder.RegisterType<WorkTaskCommentService>();
            _ = builder.RegisterType<WorkTaskService>();
            _ = builder.RegisterType<WorkTaskStatusService>();
            _ = builder.RegisterType<WorkTaskTypeService>();
        }
    }
}
