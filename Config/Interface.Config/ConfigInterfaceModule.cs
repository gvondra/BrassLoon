using Autofac;
using BrassLoon.RestClient;

namespace BrassLoon.Interface.Config
{
    public class ConfigInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<RestUtil>().SingleInstance();
            builder.RegisterType<Service>().As<IService>();
            builder.RegisterType<ItemService>().As<IItemService>();
            builder.RegisterType<LookupService>().As<ILookupService>();
        }
    }
}
