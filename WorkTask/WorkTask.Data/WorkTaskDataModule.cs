﻿using Autofac;
using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Internal;

namespace BrassLoon.WorkTask.Data
{
    public class WorkTaskDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<BrassLoon.DataClient.SqlClientProviderFactory>().As<IDbProviderFactory>();
            builder.RegisterType<WorkTaskStatusDataFactory>().As<IWorkTaskStatusDataFactory>();
            builder.RegisterType<WorkTaskStatusDataSaver>().As<IWorkTaskStatusDataSaver>();
            builder.RegisterType<WorkTaskTypeDataFactory>().As<IWorkTaskTypeDataFactory>();
            builder.RegisterType<WorkTaskTypeDataSaver>().As<IWorkTaskTypeDataSaver>();
        }
    }
}