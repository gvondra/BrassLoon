using BrassLoon.Authorization.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IEmailAddressDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, EmailAddressData data);
    }
}
