using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    internal sealed class WorkTaskEnumerator : IAsyncEnumerable<IWorkTask>, IAsyncEnumerator<IWorkTask>
    {
        private readonly Func<Task<IAsyncEnumerator<WorkTaskData>>> _getDataEnumerator;
        private readonly Func<WorkTaskData, IWorkTask> _create;
        private IAsyncEnumerator<WorkTaskData> _dataEnumerator;

        internal WorkTaskEnumerator(
            Func<Task<IAsyncEnumerator<WorkTaskData>>> getDataEnumerator,
            Func<WorkTaskData, IWorkTask> create)
        {
            _getDataEnumerator = getDataEnumerator;
            _create = create;
        }

        public IWorkTask Current { get; private set; }

        public IAsyncEnumerator<IWorkTask> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => this;

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_dataEnumerator == null)
                _dataEnumerator = await _getDataEnumerator();
            bool result = await _dataEnumerator.MoveNextAsync();
            if (result)
            {
                Current = _create(_dataEnumerator.Current);
            }
            return result;
        }

        public async ValueTask DisposeAsync()
        {
            if (_dataEnumerator != null)
                await _dataEnumerator.DisposeAsync();
        }
    }
}
