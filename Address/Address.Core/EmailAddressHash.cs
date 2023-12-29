using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Address.Core
{
    internal static class EmailAddressHash
    {
        internal static byte[] Hash(IEmailAddress emailAddress)
        {
            List<object> formattedEmail = new List<object>
            {
                new { Address = FormatAddressField(emailAddress.Address) }
            };
            string json = JsonConvert.SerializeObject(formattedEmail, BaseHash.GetSerializerSettings());
            return SHA512.HashData(Encoding.UTF8.GetBytes(json));
        }

        private static string FormatAddressField(string value)
            => Formatter.TrimAndConsolidateWhiteSpace(value).ToLower(CultureInfo.GetCultureInfo("en-us"));
    }
}
