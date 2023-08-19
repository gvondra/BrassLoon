using Autofac;
using BrassLoon.Config.Framework;

namespace BrassLoon.Config.Core
{
    public class ConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Config.Data.ConfigDataModule());
            builder.RegisterInstance(new SettingsFactory());
            builder.RegisterType<ItemFactory>().As<IItemFactory>();
            builder.RegisterType<ItemSaver>().As<IItemSaver>();
            builder.RegisterType<ItemHistoryFactory>().As<IItemHistoryFactory>();
            builder.RegisterType<LookupFactory>().As<ILookupFactory>();
            builder.RegisterType<LookupSaver>().As<ILookupSaver>();
            builder.RegisterType<LookupHistoryFactory>().As<ILookupHistoryFactory>();
        }
    }
}
