using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkGroupMemberData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkGroupMemberId { get; set; }
        [ColumnMapping] public Guid WorkGroupId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public string UserId { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
