using System;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTaskType
    {
        public Guid? WorkTaskTypeId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public int? WorkTaskCount { get; set; }
    }
}
