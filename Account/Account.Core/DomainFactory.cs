using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class DomainFactory : IDomainFactory
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly IDomainDataFactory _dataFactory;
        private readonly IDomainDataSaver _dataSaver;

        public DomainFactory(SettingsFactory settingsFactory, 
            IDomainDataFactory dataFactory, 
            IDomainDataSaver dataSaver)
        {
            _settingsFactory = settingsFactory;
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        public Task<IDomain> Create(Guid accountId)
        {
            return Task.FromResult<IDomain>(new Domain(
                new DomainData() { AccountGuid = accountId },
                _dataSaver
                )
                {
                    Deleted = false
                });
        }

        public async Task<IDomain> Get(ISettings settings, Guid id)
        {
            Domain result = null;
            DomainData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new Domain(data, _dataSaver);
            return result;
        }

        public async Task<IEnumerable<IDomain>> GetByAccountId(ISettings settings, Guid accountId)
        {
            return (await _dataFactory.GetByAccountId(_settingsFactory.CreateData(settings), accountId))
                .Where(data => !data.Deleted)
                .Select<DomainData, IDomain>(data => new Domain(data, _dataSaver));
        }

        public async Task<IEnumerable<IDomain>> GetDeletedByAccountId(ISettings settings, Guid accountId)
        {
            return (await _dataFactory.GetByAccountId(_settingsFactory.CreateData(settings), accountId))
                .Where(data => data.Deleted)
                .Select<DomainData, IDomain>(data => new Domain(data, _dataSaver));
        }
    }
}
