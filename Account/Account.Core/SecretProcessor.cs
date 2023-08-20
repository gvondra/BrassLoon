using BrassLoon.Account.Framework;
using Konscious.Security.Cryptography;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Account.Core
{
    public class SecretProcessor : ISecretProcessor
    {
        private const string PADDING = "brass-loon:";

        // this is an older method. Phasing this out
        public byte[] Hash(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException(nameof(secret));
            using (HashAlgorithm algorithm = SHA512.Create())
            {
                return algorithm.ComputeHash(
                    Encoding.Unicode.GetBytes(string.Concat(PADDING, secret.Trim()))
                    );
            }
        }

        /// <summary>
        /// Verify that the hash of 'secrect' is equal to the given 'hash'
        /// </summary>
        // this is an older method. Phasing this out
        public bool Verify(string secret, byte[] hash)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(secret) && hash != null)
            {
                byte[] secretHash = this.Hash(secret);
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
            }            
            return result;
        }

        public byte[] CreateSalt()
        {
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            random.GetBytes(salt);
            return salt;
        }

        public byte[] HashSecretArgon2i(string value, byte[] salt)
        {
            Argon2i argon = new Argon2i(Encoding.UTF8.GetBytes(value))
            {
                DegreeOfParallelism = 4,
                MemorySize = 20480,
                Iterations = 16,
                Salt = salt
            };
            return argon.GetBytes(512);
        }
    }
}
