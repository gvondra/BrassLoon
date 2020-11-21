using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserSaver
    {
        Task Create(ISettings settings, IUser user);
    }
}
