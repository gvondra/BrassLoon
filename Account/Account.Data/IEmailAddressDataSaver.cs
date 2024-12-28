using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IEmailAddressDataSaver
    {
        Task Create(ISaveSettings settings, EmailAddressData emailAddressData);
    }
}
