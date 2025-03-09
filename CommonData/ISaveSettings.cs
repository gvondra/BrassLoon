using BrassLoon.DataClient.SqlClient;

namespace BrassLoon.CommonData
{
    public interface ISaveSettings : ISqlTransactionHandler, DataClient.MongoDB.ISettings
    {
    }
}
