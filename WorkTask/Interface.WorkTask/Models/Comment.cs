using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class Comment
    {
        public Guid? CommentId { get; set; }
        public Guid? DomainId { get; set; }
        public string Text { get; set; }
        public DateTime? CreateTimestamp { get; set; }
    }
}
