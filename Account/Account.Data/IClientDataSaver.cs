using BrassLoon.Account.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientDataSaver
    {
        Task Create(ISaveSettings settings, ClientData clientData);
        Task Update(ISaveSettings settings, ClientData clientData);
    }
}
