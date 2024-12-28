using BrassLoon.DataClient;

namespace BrassLoon.CommonData
{
    public interface ISaveSettings : ISqlTransactionHandler, DataClient.MongoDB.ISettings
    {
    }
}
