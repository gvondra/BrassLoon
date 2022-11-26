using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BrassLoon.JwtUtility
{
    public static class RsaSecurityKeySerializer
    {
        public static RsaSecurityKey GetSecurityKey(string tknCsp, bool includePublicKey = false)
        {
            dynamic tknCspData = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(tknCsp)));
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
#if (NETSTANDARD2_0)
            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParameters);
            return new RsaSecurityKey(rsa.ExportParameters(includePublicKey));
#else
            return new RsaSecurityKey(RSA.Create(rsaParameters).ExportParameters(includePublicKey));
#endif
        }
    }
}
