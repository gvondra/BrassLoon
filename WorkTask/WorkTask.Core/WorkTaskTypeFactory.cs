using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        internal WorkTaskType Create(WorkTaskTypeData data) => new WorkTaskType(data, _dataSaver, this);

        public IWorkTaskType Create(Guid domainId, string code)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            return Create(
                new WorkTaskTypeData { DomainId = domainId, Code = code }
                );
        }

        public async Task<IWorkTaskType> Get(ISettings settings, Guid domainId, Guid id)
        {
            WorkTaskType workTaskType = null;
            WorkTaskTypeData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null && data.DomainId.Equals(domainId))
                workTaskType = Create(data);
            return workTaskType;
        }

        public async Task<IEnumerable<IWorkTaskType>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new DataSettings(settings), domainId))
                .Select(Create);
        }

        public IWorkTaskStatusFactory GetWorkTaskStatusFactory() => _workTaskStatusFactory;

        public async Task<IEnumerable<IWorkTaskType>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId)
        {
            return (await _dataFactory.GetByWorkGroupId(new DataSettings(settings), workGroupId))
                .Where(data => data.DomainId.Equals(domainId))
                .Select(Create);
        }

        public async Task<IWorkTaskType> GetByDomainIdCode(ISettings settings, Guid domainId, string code)
        {
            WorkTaskType workTaskType = null;
            WorkTaskTypeData data = await _dataFactory.GetByDomainIdCode(new DataSettings(settings), domainId, code);
            if (data != null)
                workTaskType = Create(data);
            return workTaskType;
        }
    }
}
