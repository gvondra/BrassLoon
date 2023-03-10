using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskContextData : DataManagedStateBase
    {
        [ColumnMapping(IsUtc = true)] public Guid WorkTaskContextId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public Guid WorkTaskId { get; set; }
        [ColumnMapping()] public short Status { get; set; }
        [ColumnMapping()] public short ReferenceType { get; set; }
        [ColumnMapping()] public string ReferenceValue { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
