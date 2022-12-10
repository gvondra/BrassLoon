using Autofac;
namespace AuthorizationAPI
{
    public class AuthorizationAPIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SettingsFactory>().SingleInstance();
        }
    }
}
