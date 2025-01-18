using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
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
