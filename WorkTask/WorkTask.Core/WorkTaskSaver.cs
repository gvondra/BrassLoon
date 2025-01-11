using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskSaver : IWorkTaskSaver
    {
        private readonly IWorkTaskDataSaver _dataSaver;

        public WorkTaskSaver(IWorkTaskDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public async Task<bool> Claim(ISettings settings, Guid domainId, Guid id, string userId, DateTime? assingedDate = null)
        {
            bool result = false;
            await Saver.Save(new TransactionHandler(settings), async (th) =>
            {
                result = await _dataSaver.Claim(th, domainId, id, userId, assingedDate);
            });
            return result;
        }

        public Task Create(ISettings settings, params IWorkTask[] workTasks)
        {
            if (workTasks != null && workTasks.Length > 0)
            {
                return Saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < workTasks.Length; i += 1)
                    {
                        await workTasks[i].Create(th);
                    }
                });
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task Update(ISettings settings, params IWorkTask[] workTasks)
        {
            if (workTasks != null && workTasks.Length > 0)
            {
                return Saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < workTasks.Length; i += 1)
                    {
                        await workTasks[i].Update(th);
                    }
                });
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
