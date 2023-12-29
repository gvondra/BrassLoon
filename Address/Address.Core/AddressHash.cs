using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.Address.Core
{
    internal static class AddressHash
    {
        internal static byte[] Hash(IAddress address)
        {
            List<object> formattedAddress = new List<object>
            {
                new { Attention = FormatAddressField(address.Attention) },
                new { Addressee = FormatAddressField(address.Addressee) },
                new { Delivery = FormatAddressField(address.Delivery) },
                new { Secondary = FormatAddressField(address.Secondary) },
                new { City = FormatAddressField(address.City) },
                new { Territory = FormatAddressField(address.Territory) },
                new { PostalCode = FormatPostalCode(address.PostalCode) },
                new { Country = FormatAddressField(address.Country) },
                new { County = FormatAddressField(address.County) }
            };
            string json = JsonConvert.SerializeObject(formattedAddress, BaseHash.GetSerializerSettings());
            return SHA512.HashData(Encoding.UTF8.GetBytes(json));
        }

        private static string FormatPostalCode(string value)
        {
            return Formatter.UnformatPostalCode(value)
                .ToLower(CultureInfo.GetCultureInfo("en-us"));
        }

        private static string FormatAddressField(string value)
            => Formatter.TrimAndConsolidateWhiteSpace(value).ToLower(CultureInfo.GetCultureInfo("en-us"));
    }
}
