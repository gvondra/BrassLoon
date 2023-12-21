using Autofac;

namespace BrassLoon.Address.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new Interface.Address.AddressInterfaceModule());

            _ = builder.RegisterType<AddressTest>();
            _ = builder.RegisterType<EmailAddressTest>();
            _ = builder.RegisterType<PhoneTest>();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
