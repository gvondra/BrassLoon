using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Address
{
    public interface IAddressService
    {
        Task<Models.Address> Get(ISettings settings, Guid domainId, Guid addressId);
        Task<Models.Address> Save(ISettings settings, Models.Address address);
    }
}
