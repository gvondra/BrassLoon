using System;
using System.Collections.Generic;

namespace BrassLoon.Interface.Authorization.Models
{
    public class User
    {
        public Guid? UserId { get; set; }
        public Guid? DomainId { get; set; }
        public string ReferenceId { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public List<AppliedRole> Roles { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }

        internal static User Create(Protos.User user)
        {
            User result = new User
            {
                CreateTimestamp = user.CreateTimestamp?.ToDateTime(),
                DomainId = !string.IsNullOrEmpty(user.DomainId) ? Guid.Parse(user.DomainId) : default(Guid?),
                EmailAddress = user.EmailAddress,
                Name = user.Name,
                ReferenceId = user.ReferenceId,
                UpdateTimestamp = user.UpdateTimestamp?.ToDateTime(),
                UserId = !string.IsNullOrEmpty(user.UserId) ? Guid.Parse(user.UserId) : default(Guid?),
                Roles = new List<AppliedRole>()
            };
            foreach (Protos.AppliedRole role in user.Roles)
            {
                result.Roles.Add(AppliedRole.Create(role));
            }
            return result;
        }
    }
}
