using BrassLoon.Config.Data;
using BrassLoon.Config.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class LookupSaver : ILookupSaver
    {
        private readonly ILookupDataSaver _dataSaver;

        public LookupSaver(ILookupDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public async Task Create(Framework.ISettings settings, ILookup lookup)
        {
            CommonCore.Saver saver = new CommonCore.Saver();
            await saver.Save(new CommonCore.TransactionHandler(settings), th => lookup.Create(new SaveSettings(settings, th)));
        }

        public async Task DeleteByCode(Framework.ISettings settings, Guid domainId, string code)
        {
            CommonCore.Saver saver = new CommonCore.Saver();
            await saver.Save(new CommonCore.TransactionHandler(settings), (th) => _dataSaver.DeleteByCode(new DataSaveSettings(new SaveSettings(settings, th)), domainId, code));
        }

        public async Task Update(Framework.ISettings settings, ILookup lookup)
        {
            CommonCore.Saver saver = new CommonCore.Saver();
            await saver.Save(new CommonCore.TransactionHandler(settings), th => lookup.Update(new SaveSettings(settings, th)));
        }
    }
}
