/*
 * This would only be used when setting up a new install or you need to replace the JWKS for some reason
 * The program create 2 files in the working directory
 *  1) a json file showing the full web key
 *  2) a txt file contianing the same except encode in base 64
 */
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RsaSecurityKeyGenerator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            RSAParameters rsaParameters;
            using (RSA serviceProvider = RSACryptoServiceProvider.Create(2048))
            {
                rsaParameters = serviceProvider.ExportParameters(true);
            }
            using (FileStream fileStream = new FileStream("rsaParameters.json", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false)))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
                        serializer.Serialize(jsonWriter, rsaParameters);
                    }
                }
            }
            RsaSecurityKey rsaSecurityKey = new RsaSecurityKey(rsaParameters);
            JsonWebKey jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaSecurityKey);
            jsonWebKey.Alg = "RS512";
            jsonWebKey.Use = "sig";
            using (FileStream fileStream = new FileStream("jwk.json", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false)))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
                        serializer.Serialize(jsonWriter, jsonWebKey);
                    }
                }
            }
            string json = JsonConvert.SerializeObject(rsaParameters, new JsonSerializerSettings { Formatting = Formatting.None, ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
            using (FileStream fileStream = new FileStream("tknCsp.txt", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false)))
                {
                    streamWriter.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));
                }
            }
            return 0;
        }
    }
}