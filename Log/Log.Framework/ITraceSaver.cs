using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface ITraceSaver
    {
        Task Create(ISettings settings, params ITrace[] traces);
    }
}
