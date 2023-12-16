#pragma warning disable IDE0130 // Namespace does not match folder structure
using System;

namespace BrassLoon.Authorization.TestClient
{
    public class AppSettings
    {
        public string AccountApiBaseAddress { get; set; }
        public string AuthorizationRpcServiceAddress { get; set; }
        public Guid? AuthorizationDomainId { get; set; }
        public string GoogleAuthorizationEndpoint { get; set; }
        public string GoogleTokenEndpoint { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure