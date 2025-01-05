using AddressRPC.Services;
using Autofac;
using BrassLoon.Address.Core;
using BrassLoon.CommonAPI;

namespace AddressRPC
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class AddressRPCModule : Module
    {
        private readonly bool _useMongoDb;

        public AddressRPCModule(bool useMongoDb)
        {
            _useMongoDb = useMongoDb;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new AddressCoreModule(_useMongoDb));
            _ = builder.RegisterType<AddressService>();
            _ = builder.RegisterType<DomainAcountAccessVerifier>().As<IDomainAcountAccessVerifier>();
            _ = builder.RegisterType<EmailAddressService>();
            _ = builder.RegisterType<MetaDataProcessor>()
                .SingleInstance()
                .As<IMetaDataProcessor>();
            _ = builder.RegisterType<MetricLogger>()
                .SingleInstance()
                .As<IMetricLogger>();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
#pragma warning restore S101 // Types should be named in PascalCase
}
