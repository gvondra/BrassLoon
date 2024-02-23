using Google.Protobuf.WellKnownTypes;
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

        internal Protos.User ToProto()
        {
            Protos.User result = new Protos.User
            {
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : null,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                UserId = UserId?.ToString("D") ?? string.Empty,
                EmailAddress = EmailAddress ?? string.Empty,
                Name = Name ?? string.Empty,
                ReferenceId = ReferenceId ?? string.Empty,
                UpdateTimestamp = UpdateTimestamp.HasValue ? Timestamp.FromDateTime(UpdateTimestamp.Value) : null
            };
            foreach (AppliedRole role in Roles ?? new List<AppliedRole>())
            {
                result.Roles.Add(role.ToProto());
            }
            return result;
        }
    }
}
