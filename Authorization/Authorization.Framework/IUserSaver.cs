using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IUserSaver
    {
        Task Create(ISettings settings, IUser user);
        Task Update(ISettings settings, IUser user);
    }
}
