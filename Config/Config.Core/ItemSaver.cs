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

        public async Task Create(ISettings settings, IItem item)
            => await Saver.Save(new SaveSettings(settings), item.Create);

        public async Task DeleteByCode(ISettings settings, Guid domainId, string code)
            => await Saver.Save(new SaveSettings(settings), ss => _dataSaver.DeleteByCode(ss, domainId, code));

        public Task Update(ISettings settings, IItem item)
            => Saver.Save(new SaveSettings(settings), item.Update);
    }
}
