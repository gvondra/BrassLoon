﻿namespace BrassLoon.Client
{
    public class AppSettings
    {
        public string GoogleAuthorizationEndpoint { get; set; }
        public string GoogleTokenEndpoint { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
        public string AccountApiBaseAddress { get; set; }
        public string LogApiBaseAddress { get; set; }
        public string ConfigApiBaseAddress { get; set; }
        public string AuthorizationApiBaseAddress { get; set; }
        public string WorkTaskApiBaseAddress { get; set; }
        public string JwksBaseAddress { get; set; }
    }
}
