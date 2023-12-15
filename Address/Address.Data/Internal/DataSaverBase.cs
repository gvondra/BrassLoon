using BrassLoon.DataClient;

namespace BrassLoon.Address.Data.Internal
{
    public abstract class DataSaverBase
    {
        private readonly IDbProviderFactory _providerFactory;

        protected DataSaverBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        protected IDbProviderFactory ProviderFactory => _providerFactory;
    }
}
