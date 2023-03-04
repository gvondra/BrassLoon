using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Models
{
    public class CommentData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid CommentId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public string Text { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
