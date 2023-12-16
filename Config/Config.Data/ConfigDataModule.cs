using Autofac;
using BrassLoon.DataClient;

namespace BrassLoon.Config.Data
{
    public class ConfigDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterInstance<ISqlDbProviderFactory>(new SqlClientProviderFactory());
            _ = builder.RegisterType<ItemDataFactory>().As<IItemDataFactory>();
            _ = builder.RegisterType<ItemDataSaver>().As<IItemDataSaver>();
            _ = builder.RegisterType<ItemHistoryDataFactory>().As<IItemHistoryDataFactory>();
            _ = builder.RegisterType<LookupDataFactory>().As<ILookupDataFactory>();
            _ = builder.RegisterType<LookupDataSaver>().As<ILookupDataSaver>();
            _ = builder.RegisterType<LookupHistoryDataFactory>().As<ILookupHistoryDataFactory>();
        }
    }
}
