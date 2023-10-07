using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BrassLoon.Client
{
    internal class AccessToken
    {
        private const string Issuer = "urn:brassloon";
        private static AccessToken _instance;
        private static string _token;
        private JwtSecurityToken _jwtSecurityToken;
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
                _jwtSecurityToken = !string.IsNullOrEmpty(value) ?  new JwtSecurityToken(value) : default;
                NotifyPropertyChanged();
            }
        }

        public string GetGoogleIdToken() => GoogleToken?["id_token"];

        public bool UserHasSysAdminAccess() => UserHasRoleAccess("sysadmin");

        public bool UserHasActAdminAccess() => UserHasRoleAccess("actadmin");

        public bool UserHasRoleAccess(string role)
        {
            return _jwtSecurityToken != null
                && _jwtSecurityToken.Claims.Any(
                    clm => string.Equals(clm.Issuer, Issuer, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(clm.Type, "role", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(clm.Value, role, StringComparison.OrdinalIgnoreCase)
                    );
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
