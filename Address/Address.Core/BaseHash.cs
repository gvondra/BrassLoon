using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BrassLoon.Address.Core
{
    internal static class BaseHash
    {
        public static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include
            };
        }
    }
}
