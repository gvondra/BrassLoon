using BrassLoon.Account.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientCredentialDataSaver
    {
        Task Create(ISaveSettings settings, ClientCredentialData clientCredentialData);
    }
}
