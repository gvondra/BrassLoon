using BrassLoon.Authorization.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IRoleDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, RoleData data);
        Task Update(CommonData.ISaveSettings settings, RoleData data);
    }
}
