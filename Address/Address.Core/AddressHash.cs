using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BrassLoon.Address.Core
{
    internal static class AddressHash
    {
        internal static byte[] Hash(IAddress address)
        {
            object formattedAddress = new
            {
                Attention = FormatAddressField(address.Attention),
                Addressee = FormatAddressField(address.Addressee),
                Delivery = FormatAddressField(address.Delivery),
                City = FormatAddressField(address.City),
                Territory = FormatAddressField(address.Territory),
                PostalCode = FormatPostalCode(address.PostalCode),
                Country = FormatAddressField(address.Country),
                County = FormatAddressField(address.County)
            };
            string json = JsonConvert.SerializeObject(formattedAddress, BaseHash.GetSerializerSettings());
            return SHA512.HashData(Encoding.UTF8.GetBytes(json));
        }

        private static string FormatPostalCode(string value)
        {
            value = FormatAddressField(value);
            return Regex.Replace(value, @"[^0-1A-Za-z]+", string.Empty, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }

        private static string FormatAddressField(string value)
        {
            value = (value ?? string.Empty).Trim().ToLower(CultureInfo.GetCultureInfo("en-us"));
            return Regex.Replace(value, @"\s{2,}", " ", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
    }
}
