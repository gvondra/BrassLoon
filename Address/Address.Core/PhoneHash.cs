using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BrassLoon.Address.Core
{
    internal static class PhoneHash
    {
        internal static byte[] Hash(IPhone phone)
        {
            object formattedPhone = new
            {
                Number = FormatAddressField(phone.Number),
                CountryCode = FormatAddressField(phone.CountryCode)
            };
            string json = JsonConvert.SerializeObject(formattedPhone, BaseHash.GetSerializerSettings());
            return SHA512.HashData(Encoding.UTF8.GetBytes(json));
        }

        private static string FormatAddressField(string value)
        {
            value = (value ?? string.Empty).Trim().ToLower(CultureInfo.GetCultureInfo("en-us"));
            return Regex.Replace(value, @"\s{2,}", " ", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
    }
}
