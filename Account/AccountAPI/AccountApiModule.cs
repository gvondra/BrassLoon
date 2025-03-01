using Autofac;
namespace AccountAPI
{
    public class AccountApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Account.Core.AccountModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            _ = builder.RegisterType<MapperFactory>().SingleInstance();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
