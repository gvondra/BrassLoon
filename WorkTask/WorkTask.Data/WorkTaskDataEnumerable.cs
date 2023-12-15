using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    internal class WorkTaskDataEnumerable : IAsyncEnumerable<WorkTaskData>
    {
        private readonly ISqlSettings _settings;
        private readonly IDbProviderFactory ProviderFactory;
        private readonly Func<DbConnection, Task<DbDataReader>> _beginReader;
        private readonly Func<DbDataReader, Task<WorkTaskData>> _loadData;

        internal WorkTaskDataEnumerable(
            ISqlSettings settings,
            IDbProviderFactory providerFactory,
            Func<DbConnection, Task<DbDataReader>> beginReader,
            Func<DbDataReader, Task<WorkTaskData>> loadData)
        {
            _settings = settings;
            ProviderFactory = providerFactory;
            _beginReader = beginReader;
            _loadData = loadData;
        }

        public IAsyncEnumerator<WorkTaskData> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new WorkTaskDataEnumerator(
                _settings,
                ProviderFactory,
                _beginReader,
                _loadData);
        }
    }
}
