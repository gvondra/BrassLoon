using Autofac;
using BrassLoon.WorkTask.TestClient.Settings;

namespace BrassLoon.WorkTask.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.WorkTask.WorkTaskInterfaceModule());
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            builder.RegisterType<WorkGroupTest>();
            builder.RegisterType<WorkTaskTest>();
            builder.RegisterType<WorkTaskPerformanceTest>();
            builder.RegisterType<WorkTaskTypeTest>();
        }
    }
}
