using BrassLoon.CommonCore;
using BrassLoon.Log.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class TraceSaver : ITraceSaver
    {
        public async Task Create(ISettings settings, params ITrace[] traces)
        {
            if (traces != null && traces.Length > 0)
            {
                SaveSettings saveSettings = new SaveSettings(settings);
                try
                {
                    for (int i = 0; i < traces.Length; i += 1)
                    {
                        await traces[i].Create(saveSettings);
                        if (saveSettings.Transaction != null)
                        {
                            saveSettings.Transaction.Commit();
                            saveSettings.Transaction.Dispose();
                            saveSettings.Transaction = null;
                        }
                    }
                }
                catch
                {
                    if (saveSettings.Transaction != null)
                    {
                        saveSettings.Transaction.Rollback();
                        saveSettings.Transaction.Dispose();
                        saveSettings.Transaction = null;
                    }
                    throw;
                }
                finally
                {
                    if (saveSettings.Connection != null)
                    {
                        await saveSettings.Connection.DisposeAsync();
                        saveSettings.Connection = null;
                    }
                }
            }
        }
    }
}
