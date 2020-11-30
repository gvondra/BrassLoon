using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Framework
{
    public interface ISecretProcessor
    {
        string Create();
        bool Verify(string secret, byte[] hash);
    }
}
