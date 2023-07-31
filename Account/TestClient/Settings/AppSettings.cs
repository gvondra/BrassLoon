using System;

namespace BrassLoon.Account.TestClient.Settings
{
    public class AppSettings
    {
        public Guid? Domain { get; set; }
        public string LogFile { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccountApiBaseAddress { get; set; }
    }
}
