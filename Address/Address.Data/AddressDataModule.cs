using Autofac;
using BrassLoon.Address.Data.Internal;
using BrassLoon.DataClient;

namespace BrassLoon.Address.Data
{
    public class AddressDataModule : Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SqlClientProviderFactory>().As<IDbProviderFactory>();
            builder.RegisterType<LoaderFactory>().As<ILoaderFactory>();
            builder.RegisterType<AddressDataFactory>().As<IAddressDataFactory>();
            builder.RegisterType<AddressDataSaver>().As<IAddressDataSaver>();
        }
    }
}
