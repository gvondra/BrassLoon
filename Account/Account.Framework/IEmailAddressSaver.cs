using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IEmailAddressSaver
    {
        Task Create(ISettings settings, IEmailAddress emailAddress);
    }
}
