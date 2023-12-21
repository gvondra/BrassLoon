using BrassLoon.Address.Framework;

namespace BrassLoon.Address.Core
{
    public sealed class EmailAddress : IEmailAddress
    {
        public Guid EmailAddressId { get; internal set; }

        public Guid DomainId { get; internal set; }

        public byte[] Hash { get; internal set; }

        public string Address { get; set; }

        public DateTime CreateTimestamp { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj is IEmailAddress emailAddress)
                return Equals(emailAddress);
            else
                return base.Equals(obj);
        }

        public bool Equals(IEmailAddress other)
        {
            bool equals = true;
            if (!DomainId.Equals(other.DomainId)
                || !StringEquals(Formatter.TrimAndConsolidateWhiteSpace(Address), Formatter.TrimAndConsolidateWhiteSpace(other.Address)))
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
            hash.Add(EmailAddressId);
            hash.Add(DomainId);
            hash.Add(Address ?? string.Empty);
            hash.Add(CreateTimestamp);
            return hash.ToHashCode();
        }
    }
}
