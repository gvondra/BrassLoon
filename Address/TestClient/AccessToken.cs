using System.Collections.Generic;

namespace BrassLoon.Address.TestClient
{
    internal sealed class AccessToken
    {
        private static readonly AccessToken _instance = new AccessToken();

        private AccessToken() { }

        public static AccessToken Get => _instance;

        public Dictionary<string, string> GoogleToken { get; set; }

        public string Token { get; set; }

        public string GetGoogleIdToken() => GoogleToken?["id_token"];
    }
}
