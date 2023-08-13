using System;

namespace BrassLoon.Config.TestClient.Settings
{
    public class AppSettings
    {
        public Guid? Domain { get; set; }
        public string LogFile { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccountApiBaseAddress { get; set; }
        public string ConfigApiBaseAddress { get; set; }
    }
}
