using System.Collections.Generic;

namespace BrassLoon.Authorization.TestClient
{
    internal sealed class AccessToken
    {
        private static readonly AccessToken _instance;
        private string _token;
        private Dictionary<string, string> _googleToken;

        static AccessToken()
        {
            _instance = new AccessToken();
        }

        private AccessToken() { }

        public static AccessToken Get => _instance;

        public Dictionary<string, string> GoogleToken
        {
            get => _googleToken;
            set => _googleToken = value;
        }

        public string Token
        {
            get => _token;
            set => _token = value;
        }

        public string GetGoogleIdToken() => GoogleToken?["id_token"];
    }
}
