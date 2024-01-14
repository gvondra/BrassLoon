using BrassLoon.DataClient;

namespace BrassLoon.Authorization.Data
{
    public abstract class DataSaverBase
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly IDbProviderFactory _providerFactory;
#pragma warning restore CA1051 // Do not declare visible instance fields

        protected DataSaverBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }
    }
}
