﻿using Autofac;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;

namespace BrassLoon.Authorization.Core
{
    public class AuthorizationCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new BrassLoon.Authorization.Data.AuthorizationDataModule());
            builder.RegisterType<Saver>().SingleInstance();
            builder.RegisterType<KeyVault>().SingleInstance();
            builder.RegisterType<ClientFactory>().As<IClientFactory>();
            builder.RegisterType<ClientSaver>().As<IClientSaver>();
            builder.RegisterType<RoleFactory>().As<IRoleFactory>();
            builder.RegisterType<RoleSaver>().As<IRoleSaver>();
            builder.RegisterType<SigningKeySaver>().As<ISigningKeySaver>();
        }
    }
}