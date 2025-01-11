using BrassLoon.Authorization.Data;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class User : IUser, DataClient.IDbTransactionObserver
    {
        private readonly UserData _data;
        private readonly IUserDataSaver _dataSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleDataSaver _roleDataSaver;
        private IEmailAddress _emailAddress;
        private bool _saveEmailAddress;
        private List<IRole> _roles;
        private List<IRole> _addRoles;
        private List<IRole> _removeRoles;

        public User(
            UserData data,
            IUserDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory,
            IRoleFactory role,
            IRoleDataSaver roleDataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
            _emailAddressFactory = emailAddressFactory;
            _roleFactory = role;
            _roleDataSaver = roleDataSaver;
        }

        public Guid UserId => _data.UserId;

        public Guid DomainId => _data.DomainId;

        public string ReferenceId => _data.ReferenceId;

        public Guid EmailAddressId { get => _data.EmailAddressId; private set => _data.EmailAddressId = value; }

        public string Name { get => _data.Name; set => _data.Name = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public async Task AddRole(Framework.ISettings settings, string policyName)
        {
            IRole role = (await _roleFactory.GetByDomainId(settings, DomainId))
                .FirstOrDefault(r => string.Equals(policyName, r.PolicyName, StringComparison.OrdinalIgnoreCase));
            if (role != null)
            {
                if (_addRoles == null)
                    _addRoles = new List<IRole>();
                _addRoles.Add(role);
            }
        }

        private async Task SaveRoleRoleChanges(ITransactionHandler transactionHandler)
        {
            if ((_addRoles != null || _removeRoles != null) && transactionHandler.Transaction != null)
                transactionHandler.Transaction.AddObserver(this);
            if (_addRoles != null)
            {
                foreach (IRole role in _addRoles)
                {
                    await _roleDataSaver.AddUserRole(transactionHandler, UserId, role.RoleId);
                }
            }
            if (_removeRoles != null)
            {
                foreach (IRole role in _removeRoles)
                {
                    await _roleDataSaver.RemoveUserRole(transactionHandler, UserId, role.RoleId);
                }
            }
        }

        public async Task Create(ITransactionHandler transactionHandler)
        {
            if (_emailAddress != null)
            {
                if (_saveEmailAddress)
                    await _emailAddress.Create(transactionHandler);
                EmailAddressId = _emailAddress.EmailAddressId;
            }
            await _dataSaver.Create(transactionHandler, _data);
            await SaveRoleRoleChanges(transactionHandler);
        }

        public async Task<IEmailAddress> GetEmailAddress(Framework.ISettings settings)
        {
            if (_emailAddress == null)
            {
                _emailAddress = await _emailAddressFactory.Get(settings, EmailAddressId);
                _saveEmailAddress = false;
            }
            return _emailAddress;
        }

        public async Task<IEnumerable<IRole>> GetRoles(Framework.ISettings settings)
        {
            if (_roles == null && !UserId.Equals(Guid.Empty))
                _roles = (await _roleFactory.GetByUserId(settings, UserId)).ToList();
            return (_roles ?? new List<IRole>())
                .Concat(_addRoles ?? new List<IRole>())
                .Where(r => _removeRoles == null || !_removeRoles.Exists(rr => r.RoleId.Equals(rr.RoleId)));
        }

        public async Task RemoveRole(Framework.ISettings settings, string policyName)
        {
            IRole role = (await _roleFactory.GetByDomainId(settings, DomainId))
                .FirstOrDefault(r => string.Equals(policyName, r.PolicyName, StringComparison.OrdinalIgnoreCase));
            if (role != null)
            {
                if (_removeRoles == null)
                    _removeRoles = new List<IRole>();
                _removeRoles.Add(role);
            }
        }

        public IEmailAddress SetEmailAddress(IEmailAddress emailAddress)
        {
            if (_emailAddress == null || _emailAddress != emailAddress)
            {
                _emailAddress = emailAddress;
                _saveEmailAddress = true;
            }
            return _emailAddress;
        }

        public async Task<IEmailAddress> SetEmailAddress(Framework.ISettings settings, string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            return SetEmailAddress(
                await _emailAddressFactory.GetByAddress(settings, address));
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            if (_emailAddress != null)
            {
                if (_saveEmailAddress)
                    await _emailAddress.Create(transactionHandler);
                EmailAddressId = _emailAddress.EmailAddressId;
            }
            await _dataSaver.Update(transactionHandler, _data);
            await SaveRoleRoleChanges(transactionHandler);
        }

        void DataClient.IDbTransactionObserver.BeforeCommit() { } // do nothing

        void DataClient.IDbTransactionObserver.AfterCommit()
        {
            // after saving roles, unset role lists to force them to reload from the DB
            _roles = null;
            _addRoles = null;
            _removeRoles = null;
        }

        void DataClient.IDbTransactionObserver.BeforeRollback() { } // do nothing

        void DataClient.IDbTransactionObserver.AfterRollback() { } // do nothing
    }
}
