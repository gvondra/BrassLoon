using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Core;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public class WorkTaskTypeFactory : IWorkTaskTypeFactory
    {
        private readonly IWorkTaskTypeDataFactory _dataFactory;
        private readonly IWorkTaskTypeDataSaver _dataSaver;

        public WorkTaskTypeFactory(IWorkTaskTypeDataFactory dataFactory, IWorkTaskTypeDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkTaskType Create(WorkTaskTypeData data) => new WorkTaskType(data, _dataSaver);

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
    }
}
