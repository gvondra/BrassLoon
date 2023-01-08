using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public interface ISettings
    {
        bool UserDefaultAzureSqlToken { get; }
        Task<string> GetConnetionString();
        Func<Task<string>> GetDatabaseAccessToken();
    }
}
