using Autofac;
using BrassLoon.Authorization.Data.Framework;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public class AuthorizationDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<BrassLoon.DataClient.SqlClientProviderFactory>().As<IDbProviderFactory>();
            builder.RegisterType<RoleDataFactory>().As<IRoleDataFactory>();
            builder.RegisterType<RoleDataSaver>().As<IRoleDataFactory>();
        }
    }
}
