using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserSaver
    {
        Task Create(ISettings settings, IUser user);
        Task Update(ISettings settings, IUser user);
    }
}
