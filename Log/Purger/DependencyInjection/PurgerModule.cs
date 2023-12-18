using Autofac;

namespace BrassLoon.Log.Purger.DependencyInjection
{
    public class PurgerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Core.LogModule());
            _ = builder.RegisterModule(new Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new Interface.Log.LogInterfaceModule());
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
