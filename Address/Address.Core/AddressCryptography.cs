using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Address.Core
{
    internal static class AddressCryptography
    {
        internal static (byte[] k, byte[] iv) CreateKey()
        {
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();
            return (aes.Key, aes.IV);
        }
        internal static string Decrypt(byte[] key, byte[] initializationVector, byte[] value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = initializationVector;
                using ICryptoTransform cryptoTransform = aes.CreateDecryptor();
                return Encoding.UTF8.GetString(
                    Transform(cryptoTransform, value));
            }
        }
        internal static byte[] Encrypt(byte[] key, byte[] initializationVector, string value)
        {
            if (value == null)
            {
                return null;
            }
            if (value.Length == 0)
            {
                return Array.Empty<byte>();
            }
            else
            {
                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = initializationVector;
                using ICryptoTransform cryptoTransform = aes.CreateEncryptor();
                return Transform(cryptoTransform, Encoding.UTF8.GetBytes(value));
            }
        }

        private static byte[] Transform(ICryptoTransform cryptoTransform, byte[] value)
        {
            using MemoryStream outputStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write);
            using (BinaryWriter writer = new BinaryWriter(cryptoStream))
            {
                writer.Write(value);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
            }
            return outputStream.ToArray();
        }
    }
}
