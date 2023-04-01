using BrassLoon.DataClient;
using System.Collections.Generic;

namespace BrassLoon.WorkTask.Data.Models
{
	public class WorkTaskData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkTaskId { get; set; }
		[ColumnMapping()] public Guid DomainId { get; set; }
		[ColumnMapping()] public Guid WorkTaskTypeId { get; set; }
		[ColumnMapping()] public Guid WorkTaskStatusId { get; set; }
		[ColumnMapping()] public string Title { get; set; }
		[ColumnMapping()] public string Text { get; set; }
		[ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
		[ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
		public WorkTaskTypeData WorkTaskType { get; set; }
		public WorkTaskStatusData WorkTaskStatus { get; set; }
		public List<WorkTaskContextData> WorkTaskContexts { get; set; }
    }
}
