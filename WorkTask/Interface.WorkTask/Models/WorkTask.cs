using System;
using System.Collections.Generic;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTask
    {
        public Guid? WorkTaskId { get; set; }
        public Guid? DomainId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public WorkTaskType WorkTaskType { get; set; }
        public WorkTaskStatus WorkTaskStatus { get; set; }
        public List<WorkTaskContext> WorkTaskContexts { get; set; }
    }
}
