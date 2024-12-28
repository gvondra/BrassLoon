using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class Domain : IDomain
    {
        private readonly DomainData _data;
        private readonly IDomainDataSaver _dataSaver;

        public Domain(
            DomainData data,
            IDomainDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid DomainId => _data.DomainGuid;

        public Guid AccountId => _data.AccountGuid;

        public string Name { get => _data.Name; set => _data.Name = value ?? string.Empty; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public bool Deleted { get => _data.Deleted; set => _data.Deleted = value; }

        public async Task Create(CommonCore.ISaveSettings saveSettings) => await _dataSaver.Create(saveSettings, _data);

        public async Task Update(CommonCore.ISaveSettings saveSettings) => await _dataSaver.Update(saveSettings, _data);
    }
}
