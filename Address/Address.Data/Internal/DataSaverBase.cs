using BrassLoon.DataClient;

namespace BrassLoon.Address.Data.Internal
{
    public abstract class DataSaverBase
    {
        protected readonly IDbProviderFactory _providerFactory;

        protected DataSaverBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }
    }
}
