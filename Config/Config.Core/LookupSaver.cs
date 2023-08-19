using BrassLoon.CommonCore;
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

        public async Task Create(ISettings settings, ILookup lookup)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), lookup.Create);
        }

        public async Task DeleteByCode(ISettings settings, Guid domainId, string code)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), (th) => _dataSaver.DeleteByCode(th, domainId, code));
        }

        public async Task Update(ISettings settings, ILookup lookup)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), lookup.Update);
        }
    }
}
