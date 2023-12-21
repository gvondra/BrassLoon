using BrassLoon.Address.Framework;

namespace BrassLoon.Address.Core
{
    public sealed class Phone : IPhone
    {
        public Guid PhoneId { get; internal set; }

        public Guid DomainId { get; internal set; }

        public byte[] Hash { get; internal set; }

        public string Number { get; set; }
        public string CountryCode { get; set; }

        public DateTime CreateTimestamp { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj is IPhone phone)
                return Equals(phone);
            else
                return base.Equals(obj);
        }

        public bool Equals(IPhone other)
        {
            bool equals = true;
            if (!DomainId.Equals(other.DomainId)
                || !StringEquals(Number, other.Number)
                || !StringEquals(CountryCode, other.CountryCode))
            {
                equals = false;
            }
            return equals;
        }

        private static bool StringEquals(string s1, string s2)
            => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(PhoneId);
            hash.Add(DomainId);
            hash.Add(Number ?? string.Empty);
            hash.Add(CountryCode ?? string.Empty);
            hash.Add(CreateTimestamp);
            return hash.ToHashCode();
        }
    }
}
