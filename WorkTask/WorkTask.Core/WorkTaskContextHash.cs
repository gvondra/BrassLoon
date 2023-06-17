using System;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.WorkTask.Core
{
    public static class WorkTaskContextHash
    {
        public static byte[] Compute(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                return sha512.ComputeHash(buffer);
            }
        }
    }
}
