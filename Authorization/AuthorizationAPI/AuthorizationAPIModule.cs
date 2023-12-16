using Autofac;
namespace AuthorizationAPI
{
    public class AuthorizationAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Authorization.Core.AuthorizationCoreModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            _ = builder.RegisterType<MapperFactory>().SingleInstance();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
