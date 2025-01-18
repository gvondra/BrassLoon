using Autofac;
using BrassLoon.CommonAPI;
using WorkTaskRPC.Services;

namespace WorkTaskRPC
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class WorkTakRPCModule : Module
    {
        private readonly bool _useMongoDb;

        public WorkTakRPCModule(bool useMongoDb)
        {
            _useMongoDb = useMongoDb;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.WorkTask.Core.WorkTaskCoreModule(_useMongoDb));
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
#pragma warning restore S101 // Types should be named in PascalCase
}
