using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IPhoneDataSaver
    {
        Task Create(ISaveSettings settings, PhoneData data);
    }
}
