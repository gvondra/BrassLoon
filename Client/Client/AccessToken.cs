using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BrassLoon.Client
{
    internal class AccessToken
    {
        private static AccessToken _instance;
        private static string _token;
        private Dictionary<string, string> _googleToken;

        static AccessToken()
        {
            _instance = new AccessToken();
        }

        private AccessToken() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public static AccessToken Get => _instance;

        public Dictionary<string, string> GoogleToken
        {
            get => _googleToken;
            set
            {
                _googleToken = value;
                NotifyPropertyChanged();
            }
        }

        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                NotifyPropertyChanged();
            }
        }

        public string GetGoogleIdToken() => GoogleToken?["id_token"];

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
