using Google.Protobuf.WellKnownTypes;
using System;

namespace BrassLoon.Interface.Authorization.Models
{
    public class Role
    {
        public Guid? RoleId { get; set; }
        public Guid? DomainId { get; set; }
        public string Name { get; set; }
        public string PolicyName { get; set; }
        public bool? IsActive { get; set; }
        public string Comment { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }

        internal static Role Create(Protos.Role role)
        {
            return new Role
            {
                Comment = role.Comment ?? string.Empty,
                CreateTimestamp = role.CreateTimestamp?.ToDateTime(),
                DomainId = !string.IsNullOrEmpty(role.DomainId) ? Guid.Parse(role.DomainId) : default(Guid?),
                IsActive = role.IsActive,
                Name = role.Name,
                PolicyName = role.PolicyName,
                RoleId = !string.IsNullOrEmpty(role.RoleId) ? Guid.Parse(role.RoleId) : default(Guid?),
                UpdateTimestamp = role.UpdateTimestamp?.ToDateTime()
            };
        }

        internal Protos.Role ToProto()
        {
            return new Protos.Role
            {
                Comment = Comment ?? string.Empty,
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : null,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                IsActive = IsActive,
                Name = Name,
                PolicyName = PolicyName,
                RoleId = RoleId?.ToString("D") ?? string.Empty,
                UpdateTimestamp = UpdateTimestamp.HasValue ? Timestamp.FromDateTime(UpdateTimestamp.Value) : null
            };
        }
    }
}
