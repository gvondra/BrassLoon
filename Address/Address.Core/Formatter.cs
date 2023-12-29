using System.Text.RegularExpressions;

namespace BrassLoon.Address.Core
{
    public static class Formatter
    {
        public static string UnformatPostalCode(string value)
        {
            value = TrimAndConsolidateWhiteSpace(value);
            return Regex.Replace(value, @"[^0-9A-Za-z]+", string.Empty, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }

        public static string TrimAndConsolidateWhiteSpace(string value)
        {
            value = (value ?? string.Empty).Trim();
            return Regex.Replace(value, @"\s{2,}", " ", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }

        public static string UnformatAddressDelivery(string value)
        {
            value = TrimAndConsolidateWhiteSpace(value);
            value = Regex.Replace(value, @"P\.?\s*O\.?\s*Box\s+", "PO Box ", RegexOptions.IgnoreCase);
            return value;
        }

        public static string UnformatPhoneNumber(string value)
        {
            value = (value ?? string.Empty).Trim();
            value = Regex.Replace(value, @"[^0-9]", string.Empty, RegexOptions.IgnoreCase);
            return value;
        }
    }
}
