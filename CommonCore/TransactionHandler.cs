using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace BrassLoon.CommonCore
{
    public class TransactionHandler : ITransactionHandler
    {
        private ISettings _settings;

        public TransactionHandler(ISettings settings)
        {
            _settings = settings;
        }

        public DbConnection Connection { get; set; }
        public DataClient.IDbTransaction Transaction { get; set; }

        public string ConnectionString => _settings.ConnectionString;
    }
}
