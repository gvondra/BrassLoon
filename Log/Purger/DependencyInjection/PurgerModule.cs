using Autofac;

namespace BrassLoon.Log.Purger.DependencyInjection
{
    public class PurgerModule : Module
    {
        private readonly bool _useMongoDb;

        public PurgerModule(bool useMongoDb)
        {
            _useMongoDb = useMongoDb;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Core.LogModule(_useMongoDb));
            _ = builder.RegisterModule(new Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new Interface.Log.LogInterfaceModule());
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
