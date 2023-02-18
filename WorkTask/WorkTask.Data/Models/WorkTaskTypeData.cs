using BrassLoon.DataClient;
using System;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskTypeData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkTaskTypeId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public string Title { get; set; }
        [ColumnMapping()] public string Description { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
        [ColumnMapping()] public int WorkTaskCount { get; set; }
    }
}
