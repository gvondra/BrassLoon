﻿using Autofac;
using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            _ = builder.RegisterModule(new Data.WorkTaskDataModule());
            _ = builder.RegisterType<Saver>().SingleInstance();
            _ = builder.RegisterType<CommentSaver>().As<ICommentSaver>();
            _ = builder.RegisterType<PurgeSaver>().As<IPurgeSaver>();
            _ = builder.RegisterType<PurgeWorkerFactory>().As<IPurgeWorkerFactory>();
            _ = builder.RegisterType<PurgeWorkerSaver>().As<IPurgeWorkerSaver>();
            _ = builder.RegisterType<WorkGroupFactory>().As<IWorkGroupFactory>();
            _ = builder.RegisterType<WorkGroupSaver>().As<IWorkGroupSaver>();
            _ = builder.RegisterType<WorkTaskCommentFactory>().As<IWorkTaskCommentFactory>();
            _ = builder.RegisterType<WorkTaskFactory>().As<IWorkTaskFactory>();
            _ = builder.RegisterType<WorkTaskPatcher>().As<IWorkTaskPatcher>();
            _ = builder.RegisterType<WorkTaskSaver>().As<IWorkTaskSaver>();
            _ = builder.RegisterType<WorkTaskStatusFactory>();
            _ = builder.RegisterType<WorkTaskStatusFactory>().As<IWorkTaskStatusFactory>();
            _ = builder.RegisterType<WorkTaskTypeFactory>();
            _ = builder.RegisterType<WorkTaskTypeFactory>().As<IWorkTaskTypeFactory>();
            _ = builder.RegisterType<WorkTaskTypeSaver>().As<IWorkTaskTypeSaver>();
        }
    }
}
