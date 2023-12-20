using Autofac;
using BrassLoon.Address.Data;
using BrassLoon.Address.Framework;
using BrassLoon.CommonCore;

namespace BrassLoon.Address.Core
{
    public class AddressCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new AddressDataModule());
            _ = builder.RegisterType<AddressFactory>();
            _ = builder.RegisterType<AddressFactory>().As<IAddressFactory>();
            _ = builder.RegisterType<AddressSaver>().As<IAddressSaver>();
            _ = builder.RegisterType<KeyVault>().As<IKeyVault>();
            _ = builder.RegisterType<Saver>();
        }
    }
}
