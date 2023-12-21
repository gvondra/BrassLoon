using BrassLoon.Address.Framework;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

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
            return Formatter.UnformatPostalCode(value)
                .ToLower(CultureInfo.GetCultureInfo("en-us"));
        }

        private static string FormatAddressField(string value)
            => Formatter.TrimAndConsolidateWhiteSpace(value).ToLower(CultureInfo.GetCultureInfo("en-us"));
    }
}
