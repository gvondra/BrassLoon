using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace BrassLoon.Account.Framework.Enumerations
{
    [Flags]
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

    public enum SecretType : short
    {
        NotSet = 0,
        SHA512 = 1, // phasing out. do not use
        Argon2 = 2
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
