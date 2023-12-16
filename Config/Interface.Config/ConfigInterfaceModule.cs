using Autofac;
using BrassLoon.RestClient;

namespace BrassLoon.Interface.Config
{
    public class ConfigInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<RestUtil>().SingleInstance();
            _ = builder.RegisterType<Service>().As<IService>();
            _ = builder.RegisterType<ItemService>().As<IItemService>();
            _ = builder.RegisterType<LookupService>().As<ILookupService>();
        }
    }
}
