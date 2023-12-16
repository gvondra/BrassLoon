using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Address.Framework
{
    public interface IAddress
    {
        Guid AddressId { get; }
        Guid DomainId { get; }
        string Attention { get; set; }
        string Addressee { get; set; }
        string Delivery { get; set; }
        string City { get; set; }
        string Territory { get; set; }
        string PostalCode { get; set; }
        string Country { get; set; }
        string County { get; set; }
        DateTime CreateTimestamp { get; }

        Task<IAddress> Save(ITransactionHandler transactionHandler);
    }
}
