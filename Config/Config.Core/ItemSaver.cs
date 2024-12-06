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

        public Task Create(Framework.ISettings settings, IItem item)
        {
            CommonCore.Saver saver = new CommonCore.Saver();
            return saver.Save(new CommonCore.TransactionHandler(settings), th => item.Create(new SaveSettings(settings, th)));
        }

        public async Task DeleteByCode(Framework.ISettings settings, Guid domainId, string code)
        {
            CommonCore.Saver saver = new CommonCore.Saver();
            await saver.Save(new CommonCore.TransactionHandler(settings), (th) => _dataSaver.DeleteByCode(new DataSaveSettings(new SaveSettings(settings, th)), domainId, code));
        }

        public Task Update(Framework.ISettings settings, IItem item)
        {
            CommonCore.Saver saver = new CommonCore.Saver();
            return saver.Save(new CommonCore.TransactionHandler(settings), th => item.Update(new SaveSettings(settings, th)));
        }
    }
}
