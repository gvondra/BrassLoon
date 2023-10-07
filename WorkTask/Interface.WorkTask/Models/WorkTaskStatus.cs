using Google.Protobuf.WellKnownTypes;
using System;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTaskStatus
    {
        public Guid? WorkTaskStatusId { get; set; }
        public Guid? WorkTaskTypeId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsDefaultStatus { get; set; }
        public bool? IsClosedStatus { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public int? WorkTaskCount { get; set; }

        internal static WorkTaskStatus Create(Protos.WorkTaskStatus workTaskStatus)
        {
            return new WorkTaskStatus
            {
                Code = workTaskStatus.Code,
                Name = workTaskStatus.Name,
                CreateTimestamp = workTaskStatus.CreateTimestamp != null ? workTaskStatus.CreateTimestamp.ToDateTime() : default,
                Description = workTaskStatus.Description,
                DomainId = !string.IsNullOrEmpty(workTaskStatus.DomainId) ? Guid.Parse(workTaskStatus.DomainId) : default,
                IsClosedStatus = workTaskStatus.IsClosedStatus,
                IsDefaultStatus = workTaskStatus.IsDefaultStatus,
                UpdateTimestamp = workTaskStatus.UpdateTimestamp != null ? workTaskStatus.UpdateTimestamp.ToDateTime() : default,
                WorkTaskCount = workTaskStatus.WorkTaskCount,
                WorkTaskStatusId = !string.IsNullOrEmpty(workTaskStatus.WorkTaskStatusId) ? Guid.Parse(workTaskStatus.WorkTaskStatusId) : default,
                WorkTaskTypeId = !string.IsNullOrEmpty(workTaskStatus.WorkTaskTypeId) ? Guid.Parse(workTaskStatus.WorkTaskTypeId) : default
            };
        }

        internal Protos.WorkTaskStatus ToProto()
        {
            return new Protos.WorkTaskStatus
            {
                Code = Code,
                Name = Name,
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : default,
                Description = Description,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                IsClosedStatus = IsClosedStatus,
                IsDefaultStatus = IsDefaultStatus,
                UpdateTimestamp = UpdateTimestamp.HasValue ? Timestamp.FromDateTime(UpdateTimestamp.Value) : default,
                WorkTaskCount = WorkTaskCount,
                WorkTaskStatusId = WorkTaskStatusId?.ToString("D") ?? string.Empty,
                WorkTaskTypeId = WorkTaskTypeId?.ToString("D") ?? string.Empty
            };
        }
    }
}
