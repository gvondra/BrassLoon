using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IClientSaver
    {
        Task Create(ISettings settings, IClient client);
        Task Update(ISettings settings, IClient client);
    }
}
