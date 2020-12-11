using System;
namespace TestClient 
{
    public sealed class Settings 
    {
        public string AccountAPIBaseAddress { get; set;}
        public string LogAPIBaseAddress { get; set;}        
        public Guid DomainId { get; set; }
        public Guid ClientId { get; set; }
        public string Secret { get; set; }
        public int EntryCount { get; set; }
    }
}