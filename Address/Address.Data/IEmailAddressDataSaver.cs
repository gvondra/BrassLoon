using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data
{
    public interface IEmailAddressDataSaver
    {
        Task Create(ISaveSettings settings, EmailAddressData data);
    }
}
