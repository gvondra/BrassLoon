using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskTypeSaver : IWorkTaskTypeSaver
    {
        private readonly Saver _saver;
        private readonly IWorkTaskStatusDataSaver _statusDataSaver;

        public WorkTaskTypeSaver(Saver saver,
            IWorkTaskStatusDataSaver statusDataSaver)
        {
            _saver = saver;
            _statusDataSaver = statusDataSaver;
        }

        public Task Create(ISettings settings, params IWorkTaskStatus[] statuses)
        {
            if (statuses != null && statuses.Length > 0)
            {
                return _saver.Save(new TransactionHandler(settings),
                    async th =>
                    {
                        for (int i = 0; i < statuses.Length; i += 1)
                        {
                            await statuses[i].Create(th);
                        }
                    });
            }
            else
            {
                return Task.CompletedTask;  
            }
        }

        public Task Create(ISettings settings, params IWorkTaskType[] types)
        {
            if (types != null && types.Length > 0)
            {
                return _saver.Save(new TransactionHandler(settings),
                    async th =>
                    {
                        for (int i = 0; i < types.Length; i += 1)
                        {
                            await types[i].Create(th);
                        }
                    });
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task DeleteStatus(ISettings settings, params Guid[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                return _saver.Save(new TransactionHandler(settings),
                    async th =>
                    {
                        for (int i = 0; i < ids.Length; i += 1)
                        {
                            await _statusDataSaver.Delete(th, ids[i]);
                        }
                    });
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task Update(ISettings settings, params IWorkTaskStatus[] statuses)
        {
            if (statuses != null && statuses.Length > 0)
            {
                return _saver.Save(new TransactionHandler(settings),
                    async th =>
                    {
                        for (int i = 0; i < statuses.Length; i += 1)
                        {
                            await statuses[i].Update(th);
                        }
                    });
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task Update(ISettings settings, params IWorkTaskType[] types)
        {
            if (types != null && types.Length > 0)
            {
                return _saver.Save(new TransactionHandler(settings),
                    async th =>
                    {
                        for (int i = 0; i < types.Length; i += 1)
                        {
                            await types[i].Update(th);
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
