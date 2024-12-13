using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    internal sealed class ClientCredential
    {
        private readonly ClientCredentialData _data;
        private readonly IClientCredentialDataSaver _dataSaver;
        private readonly IClient _client;

        public ClientCredential(
            IClient client,
            ClientCredentialData data,
            IClientCredentialDataSaver dataSaver)
        {
            _client = client;
            _data = data;
            _dataSaver = dataSaver;
        }

        private Guid ClientId { set => _data.ClientId = value; }
        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }

        public async Task Create(Framework.ISaveSettings settings)
        {
            ClientId = _client.ClientId;
            await _dataSaver.Create(new DataSaveSettings(settings), _data);
        }
    }
}
