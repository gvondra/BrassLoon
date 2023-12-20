using AddressRPC.Services;
using Autofac;
using BrassLoon.Address.Core;
using BrassLoon.CommonAPI;

namespace AddressRPC
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class AddressRPCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new AddressCoreModule());
            _ = builder.RegisterType<AddressService>();
            _ = builder.RegisterType<DomainAcountAccessVerifier>().As<IDomainAcountAccessVerifier>();
            _ = builder.RegisterType<MetaDataProcessor>()
                .SingleInstance()
                .As<IMetaDataProcessor>();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
#pragma warning restore S101 // Types should be named in PascalCase
}
