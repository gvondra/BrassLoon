using Grpc.Core;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogRPC
{
    public class MetaDataProcessor : IMetaDataProcessor
    {
        public string GetBearerAuthorizationToken(Metadata entries)
        {
            string result = string.Empty;
            Metadata.Entry entry = entries
                .FirstOrDefault(e => string.Equals(e.Key, "Authorization", StringComparison.OrdinalIgnoreCase));
            if (entry != null)
            {
                Match match = Regex.Match(entry.Value, @"^\s*Bearer\s+(\S+)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
                if (match.Success)
                {
                    result = match.Groups[1].Value;
                }
            }
            return result;
        }
    }
}
