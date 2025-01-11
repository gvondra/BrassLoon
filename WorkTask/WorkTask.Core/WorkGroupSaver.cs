﻿using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkGroupSaver : IWorkGroupSaver
    {
        private readonly IWorkTaskTypeGroupDataSaver _dataSaver;

        public WorkGroupSaver(IWorkTaskTypeGroupDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public Task Create(ISettings settings, params IWorkGroup[] workGroups)
        {
            ArgumentNullException.ThrowIfNull(workGroups);
            return Saver.Save(new TransactionHandler(settings), async th =>
            {
                for (int i = 0; i < workGroups.Length; i += 1)
                {
                    await workGroups[i].Create(th);
                }
            });
        }

        public Task CreateWorkTaskTypeGroup(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
            => Saver.Save(new TransactionHandler(settings), th => _dataSaver.Create(th, domainId, workTaskTypeId, workGroupId));

        public Task DeleteWorkTaskTypeGroup(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
            => Saver.Save(new TransactionHandler(settings), th => _dataSaver.Delete(th, domainId, workTaskTypeId, workGroupId));

        public Task Update(ISettings settings, params IWorkGroup[] workGroups)
        {
            ArgumentNullException.ThrowIfNull(workGroups);
            return Saver.Save(new TransactionHandler(settings), async th =>
            {
                for (int i = 0; i < workGroups.Length; i += 1)
                {
                    await workGroups[i].Update(th);
                }
            });
        }
    }
}
