using Autofac;

namespace BrassLoon.Interface.Address
{
    public class AddressInterfaceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<AddressService>().As<IAddressService>();
            _ = builder.RegisterType<EmailAddressService>().As<IEmailAddressService>();
        }
    }
}
