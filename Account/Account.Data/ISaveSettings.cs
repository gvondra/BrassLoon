using BrassLoon.DataClient;

namespace BrassLoon.Account.Data
{
    public interface ISaveSettings : ISqlTransactionHandler, DataClient.MongoDB.ISettings
    { }
}
