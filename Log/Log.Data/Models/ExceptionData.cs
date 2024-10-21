using BrassLoon.DataClient;
using System;

namespace BrassLoon.Log.Data.Models
{
    public class ExceptionData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public long ExceptionId { get; set; }
        [ColumnMapping] public long? ParentExceptionId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public string Message { get; set; }
        [ColumnMapping] public string TypeName { get; set; }
        [ColumnMapping] public string Source { get; set; }
        [ColumnMapping] public string AppDomain { get; set; }
        [ColumnMapping] public string TargetSite { get; set; }
        [ColumnMapping] public string StackTrace { get; set; }
        [ColumnMapping] public string Data { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping] public Guid? EventId { get; set; }
        [ColumnMapping] public string Category { get; set; }
        [ColumnMapping] public string Level { get; set; }
    }
}
