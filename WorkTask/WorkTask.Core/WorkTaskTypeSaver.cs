using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskTypeSaver : IWorkTaskTypeSaver
    {
        private readonly Saver _saver;

        public WorkTaskTypeSaver(Saver saver)
        {
            _saver = saver;
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
