using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Address.Core
{
    internal static class PhoneHash
    {
        internal static byte[] Hash(IPhone phone)
        {
            object formattedPhone = new
            {
                Number = Formatter.UnformatPhoneNumber(phone.Number),
                CountryCode = Formatter.UnformatPhoneNumber(phone.CountryCode)
            };
            string json = JsonConvert.SerializeObject(formattedPhone, BaseHash.GetSerializerSettings());
            return SHA512.HashData(Encoding.UTF8.GetBytes(json));
        }
    }
}
