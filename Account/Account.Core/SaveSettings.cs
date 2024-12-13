using BrassLoon.DataClient;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class SaveSettings : Framework.ISaveSettings
    {
        private readonly Framework.ISettings _settings;
        private readonly CommonCore.ITransactionHandler _transactionHandler;

        public SaveSettings(
            Framework.ISettings settings,
            CommonCore.ITransactionHandler transactionHandler)
        {
            _settings = settings;
            _transactionHandler = transactionHandler;
        }

        public DbConnection Connection { get => _transactionHandler.Connection; set => _transactionHandler.Connection = value; }
        public IDbTransaction Transaction { get => _transactionHandler.Transaction; set => _transactionHandler.Transaction = value; }

        public Func<Task<string>> GetAccessToken => _transactionHandler.GetAccessToken;

        public bool UseDefaultAzureToken => _transactionHandler.UseDefaultAzureToken;

        public bool UseDefaultAzureSqlToken => _transactionHandler.UseDefaultAzureToken;

        public string ClientSecretVaultAddress => _settings.ClientSecretVaultAddress;

        public Task<string> GetConnectionString() => _transactionHandler.GetConnectionString();
        public Func<Task<string>> GetDatabaseAccessToken() => _transactionHandler.GetAccessToken;
        public Task<string> GetDatabaseName() => _settings.GetDatabaseName();
    }
}
