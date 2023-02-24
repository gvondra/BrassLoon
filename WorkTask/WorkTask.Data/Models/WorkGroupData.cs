using BrassLoon.DataClient;
using System;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkGroupData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkGroupId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public string Title { get; set; }
        [ColumnMapping()] public string Description { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
