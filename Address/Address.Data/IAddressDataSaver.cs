using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IAddressDataSaver
    {
        Task Create(ISaveSettings settings, AddressData data);
    }
}
