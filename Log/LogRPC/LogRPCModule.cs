using Autofac;
namespace LogRPC
{
    public class LogRPCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
