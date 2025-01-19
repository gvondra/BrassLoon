using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;

namespace BrassLoon.WorkTask.Core
{
    public sealed class WorkTaskStatus : IWorkTaskStatus
    {
        private readonly WorkTaskStatusData _data;

        public WorkTaskStatus(WorkTaskStatusData data)
        {
            _data = data;
        }

        public Guid WorkTaskTypeId => _data.WorkTaskTypeId;

        public Guid DomainId => _data.DomainId;

        public string Name { get => _data.Name; set => _data.Name = value; }
        public string Description { get => _data.Description; set => _data.Description = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public int WorkTaskCount => _data.WorkTaskCount;

        public Guid WorkTaskStatusId => _data.WorkTaskStatusId;

        public string Code => _data.Code;

        public bool IsDefaultStatus { get => _data.IsDefaultStatus; set => _data.IsDefaultStatus = value; }
        public bool IsClosedStatus { get => _data.IsClosedStatus; set => _data.IsClosedStatus = value; }

        internal WorkTaskStatusData InnerData => _data;

        public static bool operator ==(WorkTaskStatus left, WorkTaskStatus right) => left.Equals(right);
        public static bool operator !=(WorkTaskStatus left, WorkTaskStatus right) => !left.Equals(right);

        public bool Equals(IWorkTaskStatus other)
            => ReferenceEquals(this, other) || WorkTaskStatusId == other.WorkTaskStatusId;

        public override bool Equals(object obj)
        {
            if (obj is IWorkTaskStatus other)
                return Equals(other);
            else
                return false;
        }

        public override int GetHashCode()
            => HashCode.Combine(WorkTaskStatusId);
    }
}
