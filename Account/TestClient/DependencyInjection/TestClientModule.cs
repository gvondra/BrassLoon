using Autofac;
using BrassLoon.Account.TestClient.Settings;

namespace BrassLoon.Account.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new Interface.Account.AccountInterfaceModule());
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            builder.RegisterType<CreateClientTokenTest>();
        }
    }
}
