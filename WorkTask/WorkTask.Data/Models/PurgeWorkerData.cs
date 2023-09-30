using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Models
{
    public class PurgeWorkerData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid PurgeWorkerId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public short Status { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
