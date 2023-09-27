using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface IJwksService
    {
        Task<string> GetJwks(ISettings settings, Guid domainId);
        Task<string> GetJwks(Uri address, Guid domainId);
    }
}
