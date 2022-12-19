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
            builder.RegisterType<ClientDataFactory>().As<IClientDataFactory>();
            builder.RegisterType<ClientDataSaver>().As<IClientDataSaver>();
            builder.RegisterType<EmailAddressDataFactory>().As<IEmailAddressDataFactory>();
            builder.RegisterType<EmailAddressDataSaver>().As<IEmailAddressDataSaver>();
            builder.RegisterType<RoleDataFactory>().As<IRoleDataFactory>();
            builder.RegisterType<RoleDataSaver>().As<IRoleDataSaver>();
            builder.RegisterType<SigningKeyDataFactory>().As<ISigningKeyDataFactory>();
            builder.RegisterType<SigningKeyDataSaver>().As<ISigningKeyDataSaver>();
            builder.RegisterType<UserDataFactory>().As<IUserDataFactory>();
            builder.RegisterType<UserDataSaver>().As<IUserDataSaver>();
        }
    }
}
