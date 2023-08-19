using BrassLoon.CommonCore;
using BrassLoon.Config.Data;
using BrassLoon.Config.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class ItemSaver : IItemSaver
    {
        private readonly IItemDataSaver _dataSaver;

        public ItemSaver(IItemDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public Task Create(ISettings settings, IItem item)
        {
            Saver saver = new Saver();
            return saver.Save(new TransactionHandler(settings), item.Create);
        }

        public async Task DeleteByCode(ISettings settings, Guid domainId, string code)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), (th) => _dataSaver.DeleteByCode(th, domainId, code));
        }

        public Task Update(ISettings settings, IItem item)
        {
            Saver saver = new Saver();
            return saver.Save(new TransactionHandler(settings), item.Update);
        }
    }
}
