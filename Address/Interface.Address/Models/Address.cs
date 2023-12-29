using Google.Protobuf.WellKnownTypes;
using System;

namespace BrassLoon.Interface.Address.Models
{
    public class Address
    {
        public Guid? AddressId { get; set; }
        public Guid? DomainId { get; set; }
        public string Attention { get; set; }
        public string Addressee { get; set; }
        public string Delivery { get; set; }
        public string Secondary { get; set; }
        public string City { get; set; }
        public string Territory { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public DateTime? CreateTimestamp { get; set; }

        internal static Address Create(Protos.Address address)
        {
            return new Address
            {
                AddressId = !string.IsNullOrEmpty(address.AddressId) ? Guid.Parse(address.AddressId) : default(Guid?),
                DomainId = !string.IsNullOrEmpty(address.DomainId) ? Guid.Parse(address.DomainId) : default(Guid?),
                Attention = address.Attention ?? string.Empty,
                Addressee = address.Addressee ?? string.Empty,
                Delivery = address.Delivery ?? string.Empty,
                Secondary = address.Secondary ?? string.Empty,
                City = address.City ?? string.Empty,
                Territory = address.Territory ?? string.Empty,
                PostalCode = address.PostalCode ?? string.Empty,
                Country = address.Country ?? string.Empty,
                County = address.County ?? string.Empty,
                CreateTimestamp = address.CreateTimestamp?.ToDateTime()
            };
        }

        internal Protos.Address ToProto()
        {
            return new Protos.Address
            {
                AddressId = AddressId?.ToString("D") ?? string.Empty,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                Attention = Attention ?? string.Empty,
                Addressee = Addressee ?? string.Empty,
                Delivery = Delivery ?? string.Empty,
                Secondary = Secondary ?? string.Empty,
                City = City ?? string.Empty,
                Territory = Territory ?? string.Empty,
                PostalCode = PostalCode ?? string.Empty,
                Country = Country ?? string.Empty,
                County = County ?? string.Empty,
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : default
            };
        }
    }
}
