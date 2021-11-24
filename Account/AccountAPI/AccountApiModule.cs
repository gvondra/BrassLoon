using Autofac;
namespace AccountAPI
{
    public class AccountAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Account.Core.AccountModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterInstance<SettingsFactory>(new SettingsFactory());
        }
    }
}
