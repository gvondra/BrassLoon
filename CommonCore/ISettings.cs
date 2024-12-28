using System;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public interface ISettings
    {
        bool UseDefaultAzureSqlToken { get; }
        Task<string> GetConnectionString();
        Func<Task<string>> GetDatabaseAccessToken();
        Task<string> GetDatabaseName();
    }
}
