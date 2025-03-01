using Autofac;
namespace AccountAPI
{
    public class AccountApiModule : Module
    {
        private readonly bool _useMongoDb;

        public AccountAPIModule(bool useMongoDb)
        {
            _useMongoDb = useMongoDb;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.Account.Core.AccountModule(_useMongoDb));
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            _ = builder.RegisterType<MapperFactory>().SingleInstance();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
