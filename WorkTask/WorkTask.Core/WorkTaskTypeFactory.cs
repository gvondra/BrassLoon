using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskTypeFactory : IWorkTaskTypeFactory
    {
        private readonly IWorkTaskTypeDataFactory _dataFactory;
        private readonly IWorkTaskTypeDataSaver _dataSaver;
        private readonly IWorkTaskStatusFactory _workTaskStatusFactory;

        public WorkTaskTypeFactory(IWorkTaskTypeDataFactory dataFactory, IWorkTaskTypeDataSaver dataSaver, IWorkTaskStatusFactory workTaskStatusFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _workTaskStatusFactory = workTaskStatusFactory;

        }

        private WorkTaskType Create(WorkTaskTypeData data) => new WorkTaskType(data, _dataSaver, this);

        public IWorkTaskType Create(Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            return Create(
                new WorkTaskTypeData { DomainId = domainId }
                );
        }

        public async Task<IWorkTaskType> Get(ISettings settings, Guid id)
        {
            WorkTaskType workTaskType = null;
            WorkTaskTypeData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                workTaskType = Create(data);
            return workTaskType;
        }

        public async Task<IEnumerable<IWorkTaskType>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new DataSettings(settings), domainId))
                .Select(Create);
        }

        public IWorkTaskStatusFactory GetWorkTaskStatusFactory() => _workTaskStatusFactory;

        public async Task<IEnumerable<IWorkTaskType>> GetByWorkGroupId(ISettings settings, Guid workGroupId)
        {
            return (await _dataFactory.GetByWorkGroupId(new DataSettings(settings), workGroupId))
                .Select(Create);
        }
    }
}
