using System.Threading.Tasks;

namespace BrassLoon.Address.Framework
{
    public interface IPhoneSaver
    {
        Task<IPhone> Save(ISettings settings, IPhone phone);
    }
}
