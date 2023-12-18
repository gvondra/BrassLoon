using Autofac;
using BrassLoon.Log.TestClient.Settings;

namespace BrassLoon.Log.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new Interface.Log.LogInterfaceModule());
            _ = builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            _ = builder.RegisterType<LoggerExtensionTest>();
            _ = builder.RegisterType<LoggerRPCTest>();
            _ = builder.RegisterType<LogInterfaceTest>();
        }
    }
}
