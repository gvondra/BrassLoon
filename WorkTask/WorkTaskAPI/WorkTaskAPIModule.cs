using Autofac;
namespace WorkTaskAPI
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class WorkTaskAPIModule : Module
    {
        private readonly bool _useMongoDb;

        public WorkTaskAPIModule(bool useMongoDb)
        {
            _useMongoDb = useMongoDb;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.WorkTask.Core.WorkTaskCoreModule(_useMongoDb));
            _ = builder.RegisterModule(new BrassLoon.Interface.Account.AccountInterfaceModule());
            _ = builder.RegisterModule(new BrassLoon.Interface.Log.LogInterfaceModule());
            _ = builder.RegisterType<MapperFactory>().SingleInstance();
            _ = builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
#pragma warning restore S101 // Types should be named in PascalCase
}
