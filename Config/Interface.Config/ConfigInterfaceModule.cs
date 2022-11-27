using Autofac;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
    public class ConfigInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<RestUtil>().SingleInstance();
            builder.RegisterType<Service>().As<IService>().SingleInstance();
            builder.RegisterType<ItemService>().As<IItemService>();
            builder.RegisterType<LookupService>().As<ILookupService>();
        }
    }
}
