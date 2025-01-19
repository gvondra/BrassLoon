using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskTypeSaver : IWorkTaskTypeSaver
    {
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
