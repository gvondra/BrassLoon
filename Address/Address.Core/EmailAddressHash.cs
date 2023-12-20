using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BrassLoon.Address.Core
{
    internal static class EmailAddressHash
    {
        internal static byte[] Hash(IEmailAddress emailAddress)
        {
            object formattedEmail = new
            {
                Address = FormatAddressField(emailAddress.Address)
            };
            string json = JsonConvert.SerializeObject(formattedEmail, BaseHash.GetSerializerSettings());
            return SHA512.HashData(Encoding.UTF8.GetBytes(json));
        }

        private static string FormatAddressField(string value)
        {
            value = (value ?? string.Empty).Trim().ToLower(CultureInfo.GetCultureInfo("en-us"));
            return Regex.Replace(value, @"\s{2,}", " ", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
    }
}
