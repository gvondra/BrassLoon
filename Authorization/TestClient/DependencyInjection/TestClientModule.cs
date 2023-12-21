using Autofac;

namespace BrassLoon.Authorization.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Authorization.AuthorizationInterfaceModule());
            _ = builder.RegisterType<ClientTest>();
            _ = builder.RegisterType<CreateTokenTest>();
            _ = builder.RegisterType<RoleTest>();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
            _ = builder.RegisterType<SigningKeyTest>();
            _ = builder.RegisterType<UserTest>();
        }
    }
}
