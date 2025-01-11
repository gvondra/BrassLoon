using AuthorizationRPC.Services;
using Autofac;
using BrassLoon.CommonAPI;

namespace AuthorizationRPC
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class AuthenticationRPCModule : Module
    {
        private readonly bool _useMongoDb;

        public AuthenticationRPCModule(bool useMongoDb)
        {
            _useMongoDb = useMongoDb;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Authorization.Core.AuthorizationCoreModule(_useMongoDb));
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterType<ClientService>();
            _ = builder.RegisterType<DomainAcountAccessVerifier>().As<IDomainAcountAccessVerifier>();
            _ = builder.RegisterType<JwksService>();
            _ = builder.RegisterType<MetaDataProcessor>()
                .SingleInstance()
                .As<IMetaDataProcessor>();
            _ = builder.RegisterType<RoleService>();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
            _ = builder.RegisterType<SigningKeyService>();
            _ = builder.RegisterType<TokenService>();
            _ = builder.RegisterType<UserService>();
        }
    }
#pragma warning restore S101 // Types should be named in PascalCase
}
