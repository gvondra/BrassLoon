using Autofac;
using BrassLoon.WorkTask.TestClient.Settings;

namespace BrassLoon.WorkTask.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.WorkTask.WorkTaskInterfaceModule());
            _ = builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            _ = builder.RegisterType<WorkGroupTest>();
            _ = builder.RegisterType<WorkTaskTest>();
            _ = builder.RegisterType<WorkTaskPerformanceTest>();
            _ = builder.RegisterType<WorkTaskTypeTest>();
        }
    }
}
