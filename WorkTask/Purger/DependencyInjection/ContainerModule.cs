using Autofac;

namespace BrassLoon.WorkTask.Purger.DependencyInjection
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.WorkTask.Core.WorkTaskCoreModule());
            builder.RegisterType<PurgeProcessor>();
            builder.RegisterType<SettingsFactory>()
                .SingleInstance()
                .As<ISettingsFactory>();
        }
    }
}
