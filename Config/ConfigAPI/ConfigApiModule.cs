using Autofac;
namespace ConfigAPI
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class ConfigAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
            _ = builder.RegisterModule(new BrassLoon.Config.Core.ConfigModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
        }
    }
#pragma warning restore S101 // Types should be named in PascalCase
}
