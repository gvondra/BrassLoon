using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface ISigningKeySaver
    {
        Task Create(ISettings settings, ISigningKey signingKey);
        Task Update(ISettings settings, ISigningKey signingKey);
    }
}
