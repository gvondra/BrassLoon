using Autofac;

namespace BrassLoon.Interface.Authorization
{
    public class AuthorizationInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClientService>().As<IClientService>();
            builder.RegisterType<JwksService>().As<IJwksService>();
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<SigningKeyService>().As<ISigningKeyService>();
            builder.RegisterType<TokenService>().As<ITokenService>();
            builder.RegisterType<UserService>().As<IUserService>();
        }
    }
}
