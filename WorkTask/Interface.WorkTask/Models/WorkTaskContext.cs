using Google.Protobuf.WellKnownTypes;
using System;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTaskContext
    {
        public Guid? WorkTaskContextId { get; set; }
        public Guid? DomainId { get; set; }
        public Guid? WorkTaskId { get; set; }
        public short? ReferenceType { get; set; }
        public string ReferenceValue { get; set; }
        public DateTime? CreateTimestamp { get; set; }

        internal static WorkTaskContext Create(Protos.WorkTaskContext context)
        {
            return new WorkTaskContext
            {
                CreateTimestamp = context.CreateTimestamp?.ToDateTime() ?? default,
                DomainId = !string.IsNullOrEmpty(context.DomainId) ? Guid.Parse(context.DomainId) : default,
                ReferenceType = context.ReferenceType.HasValue ? (short)context.ReferenceType.Value : default,
                ReferenceValue = context.ReferenceValue,
                WorkTaskContextId = !string.IsNullOrEmpty(context.WorkTaskContextId) ? Guid.Parse(context.WorkTaskContextId) : default,
                WorkTaskId = !string.IsNullOrEmpty(context.WorkTaskId) ? Guid.Parse(context.WorkTaskId) : default
            };
        }

        internal Protos.WorkTaskContext ToProto()
        {
            return new Protos.WorkTaskContext
            {
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : default,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                ReferenceType = ReferenceType,
                ReferenceValue = ReferenceValue,
                WorkTaskContextId = WorkTaskContextId?.ToString("D") ?? string.Empty,
                WorkTaskId = WorkTaskId?.ToString("D") ?? string.Empty
            };
        }
    }
}
