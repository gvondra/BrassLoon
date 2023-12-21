using Autofac;

namespace BrassLoon.WorkTask.Purger.DependencyInjection
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new BrassLoon.WorkTask.Core.WorkTaskCoreModule());
            _ = builder.RegisterType<PurgeProcessor>();
            _ = builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
        }
    }
}
