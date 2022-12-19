using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IRoleSaver
    {
        Task Create(ISettings settings, IRole role);
        Task Update(ISettings settings, IRole role);
    }
}
