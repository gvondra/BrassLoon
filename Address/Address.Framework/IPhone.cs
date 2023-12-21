namespace BrassLoon.Address.Framework
{
    public interface IPhone : IEquatable<IPhone>
    {
        Guid PhoneId { get; }
        Guid DomainId { get; }
        byte[] Hash { get; }
        string Number { get; set; }
        string CountryCode { get; set; }
        DateTime CreateTimestamp { get; }
    }
}
