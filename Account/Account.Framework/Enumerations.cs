using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Framework.Enumerations
{
    [Flags()]
    public enum UserRole : short
    {
        None = 0,
        SystemAdministrator = 1
    }
}
