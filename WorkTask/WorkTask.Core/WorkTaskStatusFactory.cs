﻿using BrassLoon.CommonCore;
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
    public class WorkTaskStatusFactory : IWorkTaskStatusFactory
    {
        private readonly IWorkTaskStatusDataFactory _dataFactory;
        private readonly IWorkTaskStatusDataSaver _dataSaver;

        public WorkTaskStatusFactory(IWorkTaskStatusDataFactory dataFactory, IWorkTaskStatusDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkTaskStatus Create(WorkTaskStatusData data) => new WorkTaskStatus(data, _dataSaver);
        private WorkTaskStatus Create(WorkTaskStatusData data, IWorkTaskType workTaskType) => new WorkTaskStatus(data, _dataSaver, workTaskType);

        public IWorkTaskStatus Create(IWorkTaskType workTaskType, string code)
        {
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            return Create(
                new WorkTaskStatusData { Code = code, DomainId = workTaskType.DomainId },
                workTaskType);
        }

        public async Task<IWorkTaskStatus> Get(ISettings settings, Guid id)
        {
            WorkTaskStatus workTaskStatus = null;
            WorkTaskStatusData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                workTaskStatus = Create(data);
            return workTaskStatus;
        }

        public async Task<IEnumerable<IWorkTaskStatus>> GetByWorkTaskTypeId(ISettings settings, Guid workTaskTypeId)
        {
            return (await _dataFactory.GetByWorkTaskType(new DataSettings(settings), workTaskTypeId))
                .Select(Create);
        }
    }
}