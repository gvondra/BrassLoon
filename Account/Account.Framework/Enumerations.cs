using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Framework.Enumerations
{
    [Flags()]
    public enum UserRole : short
    {
        None = 0,
        SystemAdministrator = 1,
        AccountAdministrator = 2
    }

    public enum UserInvitationStatus : short
    {
        Cancelled = -1,
        Created = 0,
        Completed = 0xFF,
    }
}
