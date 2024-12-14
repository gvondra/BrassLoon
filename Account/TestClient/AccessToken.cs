using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace BrassLoon.Account.TestClient
{
    internal sealed class AccessToken
    {
        private const string Issuer = "urn:brassloon";
        private static readonly AccessToken _instance = new AccessToken();
        private string _token;
        private JwtSecurityToken _jwtSecurityToken;

        private AccessToken() { }

        public static AccessToken Get => _instance;

        public Dictionary<string, string> GoogleToken { get; set; }

        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                _jwtSecurityToken = !string.IsNullOrEmpty(value) ? new JwtSecurityToken(value) : default;
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
                    && string.Equals(clm.Value, role, StringComparison.OrdinalIgnoreCase));
        }
    }
}
