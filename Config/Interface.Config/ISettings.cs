using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Config
{
    public interface ISettings
    {
        string BaseAddress { get; }

        Task<string> GetToken();
    }
}
