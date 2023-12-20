namespace BrassLoon.Address.Framework
{
    public interface IEmailAddress : IEquatable<IEmailAddress>
    {
        Guid EmailAddressId { get; }
        Guid DomainId { get; }
        byte[] Hash { get; }
        string Address { get; set; }
        DateTime CreateTimestamp { get; }
    }
}
