using Autofac;
using BrassLoon.Log.TestClient.Settings;

namespace BrassLoon.Log.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
            builder.RegisterType<LoggerExtensionTest>();
            builder.RegisterType<LoggerRPCTest>();
            builder.RegisterType<LogInterfaceTest>();
        }
    }
}
