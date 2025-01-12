using BrassLoon.Authorization.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IClientDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, ClientData data);
        Task Update(CommonData.ISaveSettings settings, ClientData data);
        Task AddRole(CommonData.ISaveSettings settings, ClientData data, Guid roleId);
        Task RemoveRole(CommonData.ISaveSettings settings, ClientData data, Guid roleId);
    }
}
