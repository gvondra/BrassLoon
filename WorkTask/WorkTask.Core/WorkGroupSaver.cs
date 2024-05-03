using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkGroupSaver : IWorkGroupSaver
    {
        private readonly Saver _saver;
        private readonly IWorkTaskTypeGroupDataSaver _dataSaver;

        public WorkGroupSaver(
            Saver saver,
            IWorkTaskTypeGroupDataSaver dataSaver)
        {
            _saver = saver;
            _dataSaver = dataSaver;
        }

        public Task Create(ISettings settings, params IWorkGroup[] workGroups)
        {
            ArgumentNullException.ThrowIfNull(workGroups);
            return _saver.Save(new TransactionHandler(settings), async th =>
            {
                for (int i = 0; i < workGroups.Length; i += 1)
                {
                    await workGroups[i].Create(th);
                }
            });
        }

        public Task CreateWorkTaskTypeGroup(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
            => _saver.Save(new TransactionHandler(settings), th => _dataSaver.Create(th, domainId, workTaskTypeId, workGroupId));

        public Task DeleteWorkTaskTypeGroup(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
            => _saver.Save(new TransactionHandler(settings), th => _dataSaver.Delete(th, domainId, workTaskTypeId, workGroupId));

        public Task Update(ISettings settings, params IWorkGroup[] workGroups)
        {
            ArgumentNullException.ThrowIfNull(workGroups);
            return _saver.Save(new TransactionHandler(settings), async th =>
            {
                for (int i = 0; i < workGroups.Length; i += 1)
                {
                    await workGroups[i].Update(th);
                }
            });
        }
    }
}
