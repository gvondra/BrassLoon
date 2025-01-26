using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    internal sealed class WorkTaskDataEnumerable : IAsyncEnumerable<WorkTaskData>
    {
        private readonly CommonData.ISettings _settings;
        private readonly IDbProvider _dbProvider;
        private readonly Func<IMongoCollection<WorkTaskData>, Task<IAsyncCursor<WorkTaskData>>> _beginCursor;
        private readonly Func<WorkTaskData, Task<WorkTaskData>> _initialize;

        public WorkTaskDataEnumerable(
            CommonData.ISettings settings,
            IDbProvider dbProvider,
            Func<IMongoCollection<WorkTaskData>, Task<IAsyncCursor<WorkTaskData>>> beginCursor,
            Func<WorkTaskData, Task<WorkTaskData>> initialize)
        {
            _settings = settings;
            _dbProvider = dbProvider;
            _beginCursor = beginCursor;
            _initialize = initialize;
        }

        public IAsyncEnumerator<WorkTaskData> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new WorkTaskDataEnumerator(
                _settings,
                _dbProvider,
                _beginCursor,
                _initialize);
        }
    }
}
