using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DataClient = BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Core
{
    public class WorkGroup : IWorkGroup, DataClient.IDbTransactionObserver
    {
        private readonly WorkGroupData _data;
        private readonly IWorkGroupDataSaver _dataSaver;
        private readonly IWorkGroupMemberDataSaver _workGroupMemberDataSaver;

        private List<WorkGroupMemberData> _newMemberData;
        private List<WorkGroupMemberData> _deletedMemberData;

        public WorkGroup(WorkGroupData data,
            IWorkGroupDataSaver dataSaver,
            IWorkGroupMemberDataSaver workGroupMemberDataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
            _workGroupMemberDataSaver = workGroupMemberDataSaver;

        }

        public Guid WorkGroupId => _data.WorkGroupId;

        public Guid DomainId => _data.DomainId;

        public string Title { get => _data.Title; set => _data.Title = value ?? string.Empty; }
        public string Description { get => _data.Description; set => _data.Description = value ?? string.Empty; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public IReadOnlyList<string> MemberUserIds
        {
            get
            {
                IEnumerable<WorkGroupMemberData> members = _data.Members ?? new List<WorkGroupMemberData>();
                if (_newMemberData != null)
                    members = members.Concat(_newMemberData);
                members = members.Where(m => _deletedMemberData == null || !_deletedMemberData.Any(d => string.Equals(m.UserId, d.UserId, StringComparison.OrdinalIgnoreCase)));
                return ImmutableList<string>.Empty.AddRange(
                    members
                    .Select(m => m.UserId)
                    .Distinct()
                    );
            }
        }

        public IReadOnlyList<Guid> WorkTaskTypeIds
        {
            get
            {
                IEnumerable<WorkTaskTypeGroupData> taskTypes = _data.TaskTypes ?? new List<WorkTaskTypeGroupData>();
                return ImmutableList<Guid>.Empty.AddRange(
                    taskTypes.Select(t => t.WorkTaskTypeId)
                    );
            }
        }       

        public void AddMember(string userId)
        {
            if (_data.Members == null)
                _data.Members = new List<WorkGroupMemberData>();
            if (_newMemberData == null)
                _newMemberData = new List<WorkGroupMemberData>();
            if (!_data.Members.Any(m => string.Equals(userId, m.UserId, StringComparison.OrdinalIgnoreCase)) 
                && !_newMemberData.Any(m => string.Equals(userId, m.UserId, StringComparison.OrdinalIgnoreCase)))
            {
                _newMemberData.Add(new WorkGroupMemberData { DomainId = DomainId, UserId = userId });
            }
        }

        public async Task Create(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Create(transactionHandler, _data);
            await SaveMemberChanges(transactionHandler);
        }

        public void RemoveMember(string userId)
        {
            if (_data.Members == null)
                _data.Members = new List<WorkGroupMemberData>();
            if (_deletedMemberData == null)
                _deletedMemberData = new List<WorkGroupMemberData>();
            _deletedMemberData.AddRange(_data.Members.Where(m => string.Equals(userId, m.UserId, StringComparison.OrdinalIgnoreCase) && !_deletedMemberData.Any(d => d.WorkGroupMemberId.Equals(m.WorkGroupMemberId))));
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Update(transactionHandler, _data);
            await SaveMemberChanges(transactionHandler);
        }

        void DataClient.IDbTransactionObserver.AfterCommit()
        {
            if (_data.Members == null)
                _data.Members = new List<WorkGroupMemberData>();
            if (_newMemberData != null)
            {
                foreach (WorkGroupMemberData data in _newMemberData)
                {
                    _data.Members.Add(data);
                }
                _newMemberData = new List<WorkGroupMemberData>();
            }
            if (_deletedMemberData != null)
            {
                for (int i = _data.Members.Count - 1; i >= 0; i -= 1)
                {
                    if (_deletedMemberData.Any(d => d.WorkGroupMemberId.Equals(_data.Members[i].WorkGroupMemberId)))
                        _data.Members.RemoveAt(i);
                }
                _deletedMemberData = new List<WorkGroupMemberData>();
            }
        }

        void DataClient.IDbTransactionObserver.AfterRollback() { }

        void DataClient.IDbTransactionObserver.BeforeCommit() { }

        void DataClient.IDbTransactionObserver.BeforeRollback() { }

        private async Task SaveMemberChanges(ITransactionHandler transactionHandler)
        {
            if (_data.Members == null)
                _data.Members = new List<WorkGroupMemberData>();
            if (_newMemberData != null)
            {
                foreach (WorkGroupMemberData data in _newMemberData)
                {
                    data.WorkGroupId = WorkGroupId;
                    await _workGroupMemberDataSaver.Create(transactionHandler, data);
                }
            }
            if (_deletedMemberData != null)
            {
                foreach (WorkGroupMemberData data in _deletedMemberData)
                {
                    await _workGroupMemberDataSaver.Delete(transactionHandler, data.WorkGroupMemberId);
                }
            }
        }
    }
}
