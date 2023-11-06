using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkGroup
    {
        public Guid? WorkGroupId { get; set; }
        public Guid? DomainId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public List<string> MemberUserIds { get; set; }
        public List<Guid> WorkTaskTypeIds { get; set; }

        internal static WorkGroup Create(Protos.WorkGroup workGroup)
        {
            return new WorkGroup
            {
                CreateTimestamp = workGroup.CreateTimestamp?.ToDateTime(),
                Description = workGroup.Description,
                DomainId = !string.IsNullOrEmpty(workGroup.DomainId) ? Guid.Parse(workGroup.DomainId) : default(Guid?),
                Title = workGroup.Title,
                UpdateTimestamp = workGroup.UpdateTimestamp?.ToDateTime(),
                WorkGroupId = !string.IsNullOrEmpty(workGroup.WorkGroupId) ? Guid.Parse(workGroup.WorkGroupId) : default(Guid?),
                MemberUserIds = workGroup.MemberUserIds.ToList(),
                WorkTaskTypeIds = workGroup.WorkTaskTypeIds
                    .Where(i => !string.IsNullOrEmpty(i))
                    .Select(i => Guid.Parse(i))
                    .ToList()
            };
        }

        internal Protos.WorkGroup ToProto()
        {
            Protos.WorkGroup result = new Protos.WorkGroup
            {
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : null,
                Description = Description,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                Title = Title,
                UpdateTimestamp = UpdateTimestamp.HasValue ? Timestamp.FromDateTime(UpdateTimestamp.Value) : null,
                WorkGroupId = WorkGroupId?.ToString("D") ?? string.Empty
            };
            if (MemberUserIds != null)
            {
                result.MemberUserIds.AddRange(MemberUserIds);
            }
            if (WorkTaskTypeIds != null)
            {
                result.WorkTaskTypeIds.AddRange(WorkTaskTypeIds.Select(i => i.ToString("D")));
            }
            return result;
        }
    }
}
