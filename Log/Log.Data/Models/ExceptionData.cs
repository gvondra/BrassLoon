using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Data.Models
{
    public class ExceptionData : DataManagedStateBase
    {
        [ColumnMapping("ExceptionId", IsPrimaryKey = true)] public long ExceptionId { get; set; }
        [ColumnMapping("ParentExceptionId")] public long? ParentExceptionId { get; set; }
        [ColumnMapping("DomainId")] public Guid DomainId { get; set; }
        [ColumnMapping("Message")] public string Message { get; set; }
        [ColumnMapping("TypeName")] public string TypeName { get; set; }
        [ColumnMapping("Source")] public string Source { get; set; }
        [ColumnMapping("AppDomain")] public string AppDomain { get; set; }
        [ColumnMapping("TargetSite")] public string TargetSite { get; set; }
        [ColumnMapping("StackTrace")] public string StackTrace { get; set; }
        [ColumnMapping("Data")] public string Data { get; set; }
        [ColumnMapping("CreateTimestamp")] public DateTime CreateTimestamp { get; set; }
    }
}
