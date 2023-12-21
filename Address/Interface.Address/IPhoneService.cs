using BrassLoon.Interface.Address.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Address
{
    public interface IPhoneService
    {
        Task<Phone> Get(ISettings settings, Guid domainId, Guid id);
        Task<Phone> Save(ISettings settings, Phone phone);
    }
}
