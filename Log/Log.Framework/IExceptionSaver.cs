using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IExceptionSaver
    {
        Task Create(ISettings settings, params IException[] exceptions);
    }
}
