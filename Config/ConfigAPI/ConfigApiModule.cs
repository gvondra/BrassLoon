using Autofac;
namespace ConfigAPI
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class ConfigAPIModule : Module
    {
        private readonly bool _useMongoDb;

        public ConfigAPIModule(bool useMongoDb = false)
        {
            _useMongoDb = useMongoDb;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterType<SettingsFactory>()
                .AsSelf()
                .As<ISettingsFactory>()
                .SingleInstance();
            _ = builder.RegisterModule(new BrassLoon.Config.Core.ConfigModule(_useMongoDb));
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
        }
    }
#pragma warning restore S101 // Types should be named in PascalCase
}
