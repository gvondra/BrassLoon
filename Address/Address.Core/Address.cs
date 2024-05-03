using BrassLoon.Address.Framework;

namespace BrassLoon.Address.Core
{
    public sealed class Address : IAddress
    {
        public Guid AddressId { get; internal set; }
        public Guid DomainId { get; internal set; }
        public byte[] Hash { get; internal set; }
        public string Attention { get; set; } = string.Empty;
        public string Addressee { get; set; } = string.Empty;
        public string Delivery { get; set; } = string.Empty;
        public string Secondary { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Territory { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;

        public DateTime CreateTimestamp { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj is IAddress address)
                return Equals(address);
            else
                return base.Equals(obj);
        }

        public bool Equals(IAddress other)
        {
            bool equals = true;
            if (!DomainId.Equals(other.DomainId)
                || !StringEquals(Formatter.TrimAndConsolidateWhiteSpace(Attention), Formatter.TrimAndConsolidateWhiteSpace(other.Attention))
                || !StringEquals(Formatter.TrimAndConsolidateWhiteSpace(Addressee), Formatter.TrimAndConsolidateWhiteSpace(other.Addressee))
                || !StringEquals(Formatter.UnformatAddressDelivery(Delivery), Formatter.UnformatAddressDelivery(other.Delivery))
                || !StringEquals(Formatter.UnformatAddressDelivery(Secondary), Formatter.UnformatAddressDelivery(other.Secondary))
                || !StringEquals(Formatter.TrimAndConsolidateWhiteSpace(City), Formatter.TrimAndConsolidateWhiteSpace(other.City))
                || !StringEquals(Formatter.TrimAndConsolidateWhiteSpace(Territory), Formatter.TrimAndConsolidateWhiteSpace(other.Territory))
                || !StringEquals(Formatter.UnformatPostalCode(PostalCode), Formatter.UnformatPostalCode(other.PostalCode))
                || !StringEquals(Formatter.TrimAndConsolidateWhiteSpace(Country), Formatter.TrimAndConsolidateWhiteSpace(other.Country))
                || !StringEquals(Formatter.TrimAndConsolidateWhiteSpace(County), Formatter.TrimAndConsolidateWhiteSpace(other.County)))
            {
                equals = false;
            }
            return equals;
        }

        private static bool StringEquals(string s1, string s2)
            => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
        {
            HashCode hash = default(HashCode);
            hash.Add(AddressId);
            hash.Add(DomainId);
            hash.Add(Attention);
            hash.Add(Addressee);
            hash.Add(Delivery);
            hash.Add(Secondary);
            hash.Add(City);
            hash.Add(Territory);
            hash.Add(PostalCode);
            hash.Add(Country);
            hash.Add(County);
            hash.Add(CreateTimestamp);
            return hash.ToHashCode();
        }
    }
}
