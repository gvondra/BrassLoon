using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Address.Framework
{
    public interface IAddressSaver
    {
        Task<IAddress> Save(ISettings settings, IAddress address);
    }
}
