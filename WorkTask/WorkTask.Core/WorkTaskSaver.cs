using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskSaver : IWorkTaskSaver
    {
        private readonly Saver _saver;

        public WorkTaskSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, params IWorkTask[] workTasks)
        {
            if (workTasks != null && workTasks.Length > 0)
            {
                return _saver.Save(new TransactionHandler(settings), async th =>
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
                return _saver.Save(new TransactionHandler(settings), async th =>
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
