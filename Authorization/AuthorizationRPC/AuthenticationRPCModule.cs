using AuthorizationRPC.Services;
using Autofac;
using BrassLoon.CommonAPI;

namespace AuthorizationRPC
{
    public class AuthenticationRPCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Authorization.Core.AuthorizationCoreModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterType<ClientService>();
            builder.RegisterType<DomainAcountAccessVerifier>().As<IDomainAcountAccessVerifier>();
            builder.RegisterType<JwksService>();
            builder.RegisterType<MetaDataProcessor>()
                .SingleInstance()
                .As<IMetaDataProcessor>();
            builder.RegisterType<RoleService>();
            builder.RegisterType<SettingsFactory>().SingleInstance();
            builder.RegisterType<SigningKeyService>();
            builder.RegisterType<TokenService>();
            builder.RegisterType<UserService>();
        }
    }
}
