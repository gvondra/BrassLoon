using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface ISettings : CommonCore.ISettings
    {
        Task<string> GetDatabaseName();
    }
}
