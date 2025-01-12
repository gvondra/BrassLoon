using BrassLoon.Authorization.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IUserDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, UserData data);
        Task Update(CommonData.ISaveSettings settings, UserData data);
        Task AddRole(CommonData.ISaveSettings settings, UserData data, Guid roleId);
        Task RemoveRole(CommonData.ISaveSettings settings, UserData data, Guid roleId);
    }
}
