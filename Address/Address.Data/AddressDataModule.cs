using Autofac;
using BrassLoon.Address.Data.Internal;
using BrassLoon.DataClient;

namespace BrassLoon.Address.Data
{
    public class AddressDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<BrassLoon.DataClient.SqlClientProviderFactory>().As<IDbProviderFactory>();
            builder.RegisterType<BrassLoon.DataClient.LoaderFactory>().As<BrassLoon.DataClient.ILoaderFactory>();
            builder.RegisterType<AddressDataFactory>().As<IAddressDataFactory>();
            builder.RegisterType<AddressDataSaver>().As<IAddressDataSaver>();
        }
    }
}
