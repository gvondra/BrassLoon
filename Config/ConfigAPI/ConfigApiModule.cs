using Autofac;
namespace ConfigAPI
{
    public class ConfigAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SettingsFactory>().SingleInstance();
            builder.RegisterModule(new BrassLoon.Config.Core.ConfigModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
        }
    }
}
