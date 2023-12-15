using System;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public interface ISettings
    {
        bool UseDefaultAzureSqlToken { get; }
        Task<string> GetConnetionString();
        Func<Task<string>> GetDatabaseAccessToken();
    }
}
