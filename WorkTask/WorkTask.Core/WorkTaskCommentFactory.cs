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
    public class WorkTaskCommentFactory : IWorkTaskCommentFactory
    {
        private readonly IWorkTaskCommentDataFactory _dataFactory;
        private readonly IWorkTaskCommentDataSaver _dataSaver;

        public WorkTaskCommentFactory(IWorkTaskCommentDataFactory dataFactory, IWorkTaskCommentDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkTaskComment Create(CommentData data, Guid workTaskId) => new WorkTaskComment(data, _dataSaver, workTaskId);

        public IComment Create(Guid domainId, Guid workTaskId, string text)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(workTaskId));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            return Create(new CommentData
            {
                DomainId = domainId,
                Text = text
            },
            workTaskId);
        }

        public async Task<IEnumerable<IComment>> GetByWorkTaskId(ISettings settings, Guid workTaskId)
        {
            return (await _dataFactory.GetByWorkTaskId(new DataSettings(settings), workTaskId))
                .Select<CommentData, IComment>(d => Create(d, workTaskId));
        }
    }
}
