using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Models
{
    public class WorkTaskTypeGroupData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkTaskTypeId { get; set; }
        [ColumnMapping(IsPrimaryKey = true)] public Guid WorkGroupId { get; set; }
    }
}
