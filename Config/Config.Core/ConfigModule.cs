using Autofac;
using BrassLoon.Config.Framework;

namespace BrassLoon.Config.Core
{
    public class ConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Data.ConfigDataModule());
            _ = builder.RegisterInstance(new SettingsFactory());
            _ = builder.RegisterType<ItemFactory>().As<IItemFactory>();
            _ = builder.RegisterType<ItemSaver>().As<IItemSaver>();
            _ = builder.RegisterType<ItemHistoryFactory>().As<IItemHistoryFactory>();
            _ = builder.RegisterType<LookupFactory>().As<ILookupFactory>();
            _ = builder.RegisterType<LookupSaver>().As<ILookupSaver>();
            _ = builder.RegisterType<LookupHistoryFactory>().As<ILookupHistoryFactory>();
        }
    }
}
