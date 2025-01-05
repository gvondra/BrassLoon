using Autofac;
using BrassLoon.Address.Data.Internal.SqlClient;
using BrassLoon.DataClient;

namespace BrassLoon.Address.Data
{
    public class AddressDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterGeneric(typeof(GenericDataFactory<>))
                .InstancePerLifetimeScope()
                .As(typeof(IGenericDataFactory<>));
            _ = builder.RegisterType<SqlClientProviderFactory>()
                .As<IDbProviderFactory>()
                .As<ISqlDbProviderFactory>();
            _ = builder.RegisterType<LoaderFactory>().As<ILoaderFactory>();
            _ = builder.RegisterType<AddressDataFactory>().As<IAddressDataFactory>();
            _ = builder.RegisterType<AddressDataSaver>().As<IAddressDataSaver>();
            _ = builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            _ = builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            _ = builder.RegisterType<PhoneDataFactory>().As<IPhoneDataFactory>();
            _ = builder.RegisterType<PhoneDataSaver>().As<IPhoneDataSaver>();
        }
    }
}
