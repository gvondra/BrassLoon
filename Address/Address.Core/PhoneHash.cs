using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Address.Core
{
    internal static class PhoneHash
    {
        internal static byte[] Hash(IPhone phone)
        {
            List<object> formattedPhone = new List<object>
            {
                new { Number = Formatter.UnformatPhoneNumber(phone.Number) },
                new { CountryCode = Formatter.UnformatPhoneNumber(phone.CountryCode) }
            };
            string json = JsonConvert.SerializeObject(formattedPhone, BaseHash.GetSerializerSettings());
            return SHA512.HashData(Encoding.UTF8.GetBytes(json));
        }
    }
}
