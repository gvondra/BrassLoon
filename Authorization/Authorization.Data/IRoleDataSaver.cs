using BrassLoon.Authorization.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IRoleDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, RoleData data);
        Task Update(CommonData.ISaveSettings settings, RoleData data);
        Task AddClientRole(CommonData.ISaveSettings settings, Guid clientId, Guid roleId);
        Task AddUserRole(CommonData.ISaveSettings settings, Guid userId, Guid roleId);
        Task RemoveClientRole(CommonData.ISaveSettings settings, Guid clientId, Guid roleId);
        Task RemoveUserRole(CommonData.ISaveSettings settings, Guid userId, Guid roleId);
    }
}
