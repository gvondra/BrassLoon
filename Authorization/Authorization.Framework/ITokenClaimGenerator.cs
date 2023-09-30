using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface ITokenClaimGenerator
    {
        Task<IEnumerable<Claim>> Generate(ISettings settings, IUser user);
        Task<IEnumerable<Claim>> Generate(ISettings settings, IClient client, IUser user = null);
    }
}
