using BrassLoon.Authorization.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IClientDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, ClientData data);
        Task Update(CommonData.ISaveSettings settings, ClientData data);
    }
}
