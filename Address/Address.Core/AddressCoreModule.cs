using Autofac;
using BrassLoon.CommonCore;

namespace BrassLoon.Address.Core
{
    public class AddressCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<KeyVault>().As<IKeyVault>();
        }
    }
}
