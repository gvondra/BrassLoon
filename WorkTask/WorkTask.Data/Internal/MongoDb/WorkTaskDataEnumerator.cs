using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    internal sealed class WorkTaskDataEnumerator : IAsyncEnumerator<WorkTaskData>
    {
        private readonly CommonData.ISettings _settings;
        private readonly IDbProvider _dbProvider;
        private readonly Func<IMongoCollection<WorkTaskData>, Task<IAsyncCursor<WorkTaskData>>> _beginCursor;
        private readonly Func<WorkTaskData, Task<WorkTaskData>> _initialize;
        private IMongoCollection<WorkTaskData> _collection;
        private IAsyncCursor<WorkTaskData> _cursor;
        private IEnumerator<WorkTaskData> _batchEnumerator;

        public WorkTaskDataEnumerator(
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

        public WorkTaskData Current { get; private set; }

        public async ValueTask<bool> MoveNextAsync()
        {
            bool result = true;
            if (_collection == null)
                _collection = await _dbProvider.GetCollection<WorkTaskData>(_settings, Constants.CollectionName.WorkTask);
            if (_cursor == null)
            {
                _cursor = await _beginCursor(_collection);
                result = await InnerMoveNext();
            }
            else
            {
                result = _batchEnumerator.MoveNext();
                if (!result)
                {
                    result = await InnerMoveNext();
                }
            }
            if (result)
            {
                Current = await _initialize(_batchEnumerator.Current);
            }
            return result;
        }

        private async Task<bool> InnerMoveNext()
        {
            bool result = await _cursor.MoveNextAsync();
            if (result)
            {
                _batchEnumerator = _cursor.Current.GetEnumerator();
                result = _batchEnumerator.MoveNext();
            }
            return result;
        }

        public ValueTask DisposeAsync()
        {
            _batchEnumerator = null;
            _cursor?.Dispose();
            _cursor = null;
            _collection = null;
            return ValueTask.CompletedTask;
        }
    }
}
