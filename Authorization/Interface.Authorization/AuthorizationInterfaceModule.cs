using Autofac;

namespace BrassLoon.Interface.Authorization
{
    public class AuthorizationInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<ClientService>().As<IClientService>();
            _ = builder.RegisterType<JwksService>().As<IJwksService>();
            _ = builder.RegisterType<RoleService>().As<IRoleService>();
            _ = builder.RegisterType<SigningKeyService>().As<ISigningKeyService>();
            _ = builder.RegisterType<TokenService>().As<ITokenService>();
            _ = builder.RegisterType<UserService>().As<IUserService>();
        }
    }
}
