using System;

namespace BrassLoon.Log.TestClient.Settings
{
    public sealed class AppSettings
    {
        public string AccountAPIBaseAddress { get; set; }
        public string LogAPIBaseAddress { get; set; }
        public Guid DomainId { get; set; }
        public Guid ClientId { get; set; }
        public string Secret { get; set; }
        public int EntryCount { get; set; }
        public int ConcurentTaskCount { get; set; }
    }
}