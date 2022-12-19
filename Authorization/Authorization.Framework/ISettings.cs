using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface ISettings : BrassLoon.CommonCore.ISettings
    {
        string SigningKeyVaultAddress { get; }
        string ClientSecretVaultAddress { get; }
    }
}
