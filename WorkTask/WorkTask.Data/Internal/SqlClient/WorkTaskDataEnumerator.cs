using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    internal sealed class WorkTaskDataEnumerator : IAsyncEnumerator<WorkTaskData>
    {
        private readonly ISqlSettings _settings;
        private readonly IDbProviderFactory _providerFactory;
        private readonly Func<DbConnection, Task<DbDataReader>> _beginReader;
        private readonly Func<DbDataReader, Task<WorkTaskData>> _loadData;
        private DbConnection _connection;
        private DbDataReader _reader;

        internal WorkTaskDataEnumerator(
            CommonData.ISettings settings,
            IDbProviderFactory providerFactory,
            Func<DbConnection, Task<DbDataReader>> beginReader,
            Func<DbDataReader, Task<WorkTaskData>> loadData)
        {
            _settings = settings;
            _providerFactory = providerFactory;
            _beginReader = beginReader;
            _loadData = loadData;
        }

        public WorkTaskData Current { get; private set; }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_connection == null)
                _connection = await _providerFactory.OpenConnection(_settings);
            if (_reader == null)
                _reader = await _beginReader(_connection);
            bool result = await _reader.ReadAsync();
            if (result)
                Current = await _loadData(_reader);
            return result;
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
            if (_reader != null)
            {
                _reader.Close();
                await _reader.DisposeAsync();
                _reader = null;
            }
        }
    }
}
