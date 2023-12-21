using Autofac;
namespace LogAPI
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class LogAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<MapperFactory>().SingleInstance();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
            _ = builder.RegisterModule(new BrassLoon.Log.Core.LogModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
        }
    }
#pragma warning restore S101 // Types should be named in PascalCase
}
