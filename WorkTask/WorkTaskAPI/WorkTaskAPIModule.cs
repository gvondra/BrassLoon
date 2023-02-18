using Autofac;
namespace WorkTaskAPI
{
    public class WorkTaskAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.WorkTask.Core.WorkTaskCoreModule());
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            builder.RegisterType<MapperFactory>().SingleInstance();
            builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
