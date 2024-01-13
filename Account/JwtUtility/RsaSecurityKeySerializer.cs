using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.JwtUtility
{
    public static class RsaSecurityKeySerializer
    {
        public static RsaSecurityKey GetSecurityKey(string tknCsp, bool includePrivateKey = false)
        {
            dynamic tknCspData = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(tknCsp)), CreateJsonSerializerSettings());
            RSAParameters rsaParameters = new RSAParameters
            {
                D = Base64UrlEncoder.DecodeBytes((string)tknCspData.d),
                DP = Base64UrlEncoder.DecodeBytes((string)tknCspData.dp),
                DQ = Base64UrlEncoder.DecodeBytes((string)tknCspData.dq),
                Exponent = Base64UrlEncoder.DecodeBytes((string)tknCspData.exponent),
                InverseQ = Base64UrlEncoder.DecodeBytes((string)tknCspData.inverseQ),
                Modulus = Base64UrlEncoder.DecodeBytes((string)tknCspData.modulus),
                P = Base64UrlEncoder.DecodeBytes((string)tknCspData.p),
                Q = Base64UrlEncoder.DecodeBytes((string)tknCspData.q)
            };
#if NETSTANDARD2_0
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(rsaParameters);
                return new RsaSecurityKey(rsa.ExportParameters(includePrivateKey));
            }
#else
            using (RSA rsa = RSA.Create(rsaParameters))
            {
                return new RsaSecurityKey(rsa.ExportParameters(includePrivateKey));
            }
#endif
        }

        public static string Serialize(RSAParameters rsaParameters)
        {
            string json = JsonConvert.SerializeObject(rsaParameters, CreateJsonSerializerSettings());
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private static JsonSerializerSettings CreateJsonSerializerSettings()
            => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
    }
}
