using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class WorkTask
    {
        public Guid? WorkTaskId { get; set; }
        public Guid? DomainId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AssignedToUserId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public WorkTaskType WorkTaskType { get; set; }
        public WorkTaskStatus WorkTaskStatus { get; set; }
        public List<WorkTaskContext> WorkTaskContexts { get; set; }

        internal static WorkTask Create(Protos.WorkTask workTask)
        {
            WorkTask result = new WorkTask
            {
                AssignedDate = !string.IsNullOrEmpty(workTask.AssignedDate) ? DateTime.Parse(workTask.AssignedDate, CultureInfo.InvariantCulture) : default(DateTime?),
                AssignedToUserId = workTask.AssignedToUserId,
                ClosedDate = !string.IsNullOrEmpty(workTask.ClosedDate) ? DateTime.Parse(workTask.ClosedDate, CultureInfo.InvariantCulture) : default(DateTime?),
                CreateTimestamp = workTask.CreateTimestamp?.ToDateTime(),
                DomainId = !string.IsNullOrEmpty(workTask.DomainId) ? Guid.Parse(workTask.DomainId) : default(Guid?),
                Text = workTask.Text,
                Title = workTask.Title,
                UpdateTimestamp = workTask.UpdateTimestamp?.ToDateTime(),
                WorkTaskId = !string.IsNullOrEmpty(workTask.WorkTaskId) ? Guid.Parse(workTask.WorkTaskId) : default(Guid?),
                WorkTaskStatus = workTask.WorkTaskStatus != null ? WorkTaskStatus.Create(workTask.WorkTaskStatus) : null,
                WorkTaskType = workTask.WorkTaskType != null ? WorkTaskType.Create(workTask.WorkTaskType) : null,
                WorkTaskContexts = new List<WorkTaskContext>(workTask.WorkTaskContexts.Select(c => WorkTaskContext.Create(c)))
            };
            return result;
        }

        internal Protos.WorkTask ToProto()
        {
            Protos.WorkTask result = new Protos.WorkTask
            {
                AssignedDate = AssignedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                AssignedToUserId = AssignedToUserId,
                ClosedDate = ClosedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : default,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                Text = Text,
                Title = Title,
                UpdateTimestamp = UpdateTimestamp.HasValue ? Timestamp.FromDateTime(UpdateTimestamp.Value) : default,
                WorkTaskId = WorkTaskId?.ToString("D") ?? string.Empty,
                WorkTaskStatus = WorkTaskStatus != null ? WorkTaskStatus.ToProto() : default,
                WorkTaskType = WorkTaskType != null ? WorkTaskType.ToProto() : default
            };
            if (WorkTaskContexts != null)
            {
                result.WorkTaskContexts.AddRange(WorkTaskContexts.Select(c => c.ToProto()));
            }
            return result;
        }
    }
}
