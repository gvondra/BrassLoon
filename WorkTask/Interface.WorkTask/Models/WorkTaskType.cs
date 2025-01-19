using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTaskType
    {
        public Guid? WorkTaskTypeId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public short? PurgePeriod { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public int? WorkTaskCount { get; set; }
        public List<WorkTaskStatus> Statuses { get; set; }

        internal static WorkTaskType Create(Protos.WorkTaskType proto)
        {
            return new WorkTaskType
            {
                Code = proto.Code,
                Title = proto.Title,
                Description = proto.Description,
                CreateTimestamp = proto.CreateTimestamp?.ToDateTime(),
                DomainId = !string.IsNullOrEmpty(proto.DomainId) ? Guid.Parse(proto.DomainId) : default(Guid?),
                PurgePeriod = proto.PurgePeriod.HasValue ? (short)proto.PurgePeriod.Value : default(short?),
                UpdateTimestamp = proto.UpdateTimestamp?.ToDateTime(),
                WorkTaskCount = proto.WorkTaskCount,
                WorkTaskTypeId = !string.IsNullOrEmpty(proto.WorkTaskTypeId) ? Guid.Parse(proto.WorkTaskTypeId) : default(Guid?),
                Statuses = proto.Statuses?.Select(WorkTaskStatus.Create).ToList() ?? new List<WorkTaskStatus>()
            };
        }

        internal Protos.WorkTaskType ToProto()
        {
            Protos.WorkTaskType workTaskType = new Protos.WorkTaskType
            {
                Code = Code,
                Title = Title,
                Description = Description,
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : null,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                PurgePeriod = PurgePeriod,
                UpdateTimestamp = UpdateTimestamp.HasValue ? Timestamp.FromDateTime(UpdateTimestamp.Value) : null,
                WorkTaskCount = WorkTaskCount,
                WorkTaskTypeId = WorkTaskTypeId?.ToString("D") ?? string.Empty
            };
            if (Statuses != null)
            {
                workTaskType.Statuses.AddRange(Statuses.Select(sts => sts.ToProto()));
            }
            return workTaskType;
        }
    }
}
