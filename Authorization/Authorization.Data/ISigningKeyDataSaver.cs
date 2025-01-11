using BrassLoon.Authorization.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface ISigningKeyDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, SigningKeyData data);
        Task Update(CommonData.ISaveSettings settings, SigningKeyData data);
    }
}
