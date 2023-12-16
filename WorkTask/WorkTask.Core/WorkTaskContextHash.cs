using System;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.WorkTask.Core
{
    public static class WorkTaskContextHash
    {
        public static byte[] Compute(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            return SHA512.HashData(buffer);
        }
    }
}
