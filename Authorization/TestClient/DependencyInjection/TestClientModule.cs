using Autofac;

namespace BrassLoon.Authorization.TestClient.DependencyInjection
{
    public class TestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Authorization.AuthorizationInterfaceModule());
            builder.RegisterType<ClientTest>();
            builder.RegisterType<CreateTokenTest>();
            builder.RegisterType<RoleTest>();
            builder.RegisterType<SettingsFactory>().SingleInstance();
            builder.RegisterType<SigningKeyTest>();
        }
    }
}
