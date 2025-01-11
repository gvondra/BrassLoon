using BrassLoon.DataClient;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public abstract class DataSaverBase
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Fields should be private
        protected readonly IDbProviderFactory _providerFactory;
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CA1051 // Do not declare visible instance fields

        protected DataSaverBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }
    }
}
