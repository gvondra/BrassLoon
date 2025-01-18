using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskTypeSaver : IWorkTaskTypeSaver
    {
        private readonly IWorkTaskStatusDataSaver _statusDataSaver;

        public WorkTaskTypeSaver(IWorkTaskStatusDataSaver statusDataSaver)
        {
            _statusDataSaver = statusDataSaver;
        }

        public Task Create(ISettings settings, params IWorkTaskStatus[] statuses)
        {
            if (statuses != null && statuses.Length > 0)
            {
                return Saver.Save(
                    new SaveSettings(settings),
                    async ss =>
                    {
                        for (int i = 0; i < statuses.Length; i += 1)
                        {
                            await statuses[i].Create(ss);
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
                return Saver.Save(
                    new SaveSettings(settings),
                    async ss =>
                    {
                        for (int i = 0; i < types.Length; i += 1)
                        {
                            await types[i].Create(ss);
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
                return Saver.Save(
                    new SaveSettings(settings),
                    async ss =>
                    {
                        for (int i = 0; i < ids.Length; i += 1)
                        {
                            await _statusDataSaver.Delete(ss, ids[i]);
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
                return Saver.Save(
                    new SaveSettings(settings),
                    async ss =>
                    {
                        for (int i = 0; i < statuses.Length; i += 1)
                        {
                            await statuses[i].Update(ss);
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
                return Saver.Save(
                    new SaveSettings(settings),
                    async ss =>
                    {
                        for (int i = 0; i < types.Length; i += 1)
                        {
                            await types[i].Update(ss);
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
