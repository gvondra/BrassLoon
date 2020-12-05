using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public interface ISettings
    {
        Task<string> GetConnetionString();
    }
}
