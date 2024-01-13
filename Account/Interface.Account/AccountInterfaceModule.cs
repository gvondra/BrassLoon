using Autofac;
using BrassLoon.RestClient;

namespace BrassLoon.Interface.Account
{
    public class AccountInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<RestUtil>();
            _ = builder.RegisterType<Service>().As<IService>();
            _ = builder.RegisterType<AccountService>().As<IAccountService>();
            _ = builder.RegisterType<ClientService>().As<IClientService>();
            _ = builder.RegisterType<DomainService>().As<IDomainService>();
            _ = builder.RegisterType<TokenService>().As<ITokenService>();
            _ = builder.RegisterType<UserInvitationService>().As<IUserInvitationService>();
            _ = builder.RegisterType<UserService>().As<IUserService>();
        }
    }
}
