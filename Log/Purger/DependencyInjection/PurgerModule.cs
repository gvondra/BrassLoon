using Autofac;

namespace BrassLoon.Log.Purger.DependencyInjection
{
    public class PurgerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Log.Core.LogModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
