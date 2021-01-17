using Autofac;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Config.Data
{
    public class ConfigDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance<ISqlDbProviderFactory>(new SqlClientProviderFactory());
            builder.RegisterType<ItemDataFactory>().As<IItemDataFactory>();
            builder.RegisterType<ItemDataSaver>().As<IItemDataSaver>();
            builder.RegisterType<ItemHistoryDataFactory>().As<IItemHistoryDataFactory>();
            builder.RegisterType<LookupDataFactory>().As<ILookupDataFactory>();
            builder.RegisterType<LookupDataSaver>().As<ILookupDataSaver>();
            builder.RegisterType<LookupHistoryDataFactory>().As<ILookupHistoryDataFactory>();
        }
    }
}
