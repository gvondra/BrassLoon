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
            _ = builder.RegisterType<SqlClientProviderFactory>().As<IDbProviderFactory>();
            _ = builder.RegisterType<LoaderFactory>().As<ILoaderFactory>();
            _ = builder.RegisterType<AddressDataFactory>().As<IAddressDataFactory>();
            _ = builder.RegisterType<AddressDataSaver>().As<IAddressDataSaver>();
            _ = builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
        }
    }
}
