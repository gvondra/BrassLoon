using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Account.Core
{
    public class SecretProcessor
    {
        private const string PADDING = "brass-loon:";
        public byte[] Hash(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException(nameof(secret));
            using (HashAlgorithm algorithm = new SHA512Managed())
            {
                return algorithm.ComputeHash(
                    Encoding.Unicode.GetBytes(string.Concat(PADDING, secret.Trim()))
                    );
            }
        }

        /// <summary>
        /// Verify that the hash of 'secrect' is equal to the given 'hash'
        /// </summary>
        public bool Verify(string secret, byte[] hash)
        {
            byte[] secretHash = this.Hash(secret);
            bool result = false;
            if (secretHash.Length == hash.Length)
            {
                bool isMatch = true;
                int i = 0;
                while (isMatch && i < hash.Length)
                {
                    if (secretHash[i] != hash[i])
                    {
                        isMatch = false;
                    }
                    i += 1;
                }
                result = isMatch;
            }
            return result;
        }

        public string Create()
        {
            return string.Concat(
                Guid.NewGuid().ToString("N"),
                Guid.NewGuid().ToString("N")
                );
        }
    }
}
