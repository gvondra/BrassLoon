using Autofac;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class AuthorizationCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Authorization.Data.AuthorizationDataModule());
            builder.RegisterType<Saver>().SingleInstance();
            builder.RegisterType<RoleFactory>().As<IRoleFactory>();
            builder.RegisterType<RoleSaver>().As<IRoleSaver>();
        }
    }
}
