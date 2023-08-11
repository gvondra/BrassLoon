using Autofac;
namespace LogRPC
{
    public class LogRPCModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            builder.RegisterModule(new BrassLoon.Log.Core.LogModule());
            builder.RegisterType<DomainAcountAccessVerifier>().As<IDomainAcountAccessVerifier>();
            builder.RegisterType<MetaDataProcessor>()
                .SingleInstance()
                .As<IMetaDataProcessor>();
            builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
