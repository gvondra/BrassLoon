using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.LoggingTest
{
    public class Settings
    {
        public string AccountApiBaseAddress { get; set; }
        public string LogApiBaseAddress { get; set; }
        public Guid LogDomainId { get; set; }
        public Guid LogClientId { get; set; }
        public string LogClientSecret { get; set; }
    }
}
