using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class Role : IRole
    {
        private readonly RoleData _data;
        private readonly IRoleDataSaver _dataSaver;

        public Role(RoleData data, IRoleDataSaver roleDataSaver)
        {
            _data = data;
            _dataSaver = roleDataSaver;
        }

        public Guid RoleId => _data.RoleId;

        public Guid DomainId => _data.DomainId;

        public string Name { get => _data.Name ?? string.Empty; set => _data.Name = (value ?? string.Empty).Trim(); }

        public string PolicyName => _data.PolicyName;

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }
        public string Comment { get => _data.Comment ?? string.Empty; set => _data.Comment = (value ?? string.Empty).TrimEnd(); }

        public Task Create(ITransactionHandler transactionHandler)
        => _dataSaver.Create(transactionHandler, _data);

        public Task Update(ITransactionHandler transactionHandler)
        => _dataSaver.Update(transactionHandler, _data);
    }
}
