﻿using System;
using System.Collections.Generic;

namespace BrassLoon.Interface.Authorization.Models
{
    public class Client
    {
        public Guid? ClientId { get; set; }
        public Guid? DomainId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserName { get; set; }
        public string Secret { get; set; }
        public List<AppliedRole> Roles { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
