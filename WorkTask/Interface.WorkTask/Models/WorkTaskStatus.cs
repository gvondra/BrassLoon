using System;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTaskStatus
    {
        public Guid? WorkTaskStatusId { get; set; }
        public Guid? WorkTaskTypeId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsDefaultStatus { get; set; }
        public bool? IsClosedStatus { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public int? WorkTaskCount { get; set; }
    }
}
