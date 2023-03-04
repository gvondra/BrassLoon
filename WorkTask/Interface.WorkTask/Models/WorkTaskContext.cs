using System;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTaskContext
    {
        public Guid? WorkTaskContextId { get; set; }
        public Guid? DomainId { get; set; }
        public Guid? WorkTaskId { get; set; }
        public short? ReferenceType { get; set; }
        public string ReferenceValue { get; set; }
        public DateTime? CreateTimestamp { get; set; }
    }
}
