using BrassLoon.Authorization.Framework;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Authorization.Core
{
    public class SecretGenerator : ISecretGenerator
    {
        public string GenerateSecret()
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            byte firstCharacter = 33;
            byte characterRange = 94;
            byte[] values = new byte[32];
            randomNumberGenerator.GetBytes(values);
            for (int i = 0; i < values.Length; i += 1)
            {
                values[i] = (byte)((values[i] % characterRange) + firstCharacter);
            }
            return Encoding.ASCII.GetString(values);
        }
    }
}
