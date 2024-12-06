using BrassLoon.DataClient;

namespace BrassLoon.Config.Data
{
    public interface ISaveSettings : ISqlTransactionHandler, DataClient.MongoDB.ISettings
    { }
}
