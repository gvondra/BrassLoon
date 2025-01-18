using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    internal sealed class WorkTaskDataEnumerable : IAsyncEnumerable<WorkTaskData>
    {
        private readonly CommonData.ISettings _settings;
        private readonly IDbProviderFactory _providerFactory;
        private readonly Func<DbConnection, Task<DbDataReader>> _beginReader;
        private readonly Func<DbDataReader, Task<WorkTaskData>> _loadData;

        internal WorkTaskDataEnumerable(
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

        public IAsyncEnumerator<WorkTaskData> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new WorkTaskDataEnumerator(
                _settings,
                _providerFactory,
                _beginReader,
                _loadData);
        }
    }
}
