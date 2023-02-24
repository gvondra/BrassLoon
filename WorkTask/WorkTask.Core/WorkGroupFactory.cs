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
    public class WorkGroupFactory : IWorkGroupFactory
    {
        private readonly IWorkGroupDataFactory _dataFactory;
        private readonly IWorkGroupDataSaver _dataSaver;

        public WorkGroupFactory(IWorkGroupDataFactory dataFactory,
            IWorkGroupDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkGroup Create(WorkGroupData data) => new WorkGroup(data, _dataSaver);

        public IWorkGroup Create(Guid domainId)
        {
            return Create(
                new WorkGroupData
                {
                    DomainId = domainId
                });
        }

        public async Task<IWorkGroup> Get(ISettings settings, Guid id)
        {
            WorkGroup result = null;
            WorkGroupData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                result = Create(data);
            return result;
        }

        public async Task<IEnumerable<IWorkGroup>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new DataSettings(settings), domainId))
                .Select<WorkGroupData, IWorkGroup>(Create);
        }
    }
}
