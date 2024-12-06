using Autofac;
using BrassLoon.Config.TestClient.Settings;

namespace BrassLoon.Config.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Config.ConfigInterfaceModule());
            _ = builder.RegisterType<ItemTest>();
            _ = builder.RegisterType<LookupTest>();
            _ = builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
        }
    }
}
