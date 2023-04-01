using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskStatusData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkTaskStatusId { get; set; }
		[ColumnMapping()] public Guid WorkTaskTypeId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public string Code { get; set; }
        [ColumnMapping()] public string Name { get; set; }
        [ColumnMapping()] public string Description { get; set; }
        [ColumnMapping()] public bool IsDefaultStatus { get; set; }
        [ColumnMapping()] public bool IsClosedStatus { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping()] public int WorkTaskCount { get; set; }
    }
}
