using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Internal
{
    public abstract class DataSaverBase
    {
        protected readonly IDbProviderFactory _providerFactory;

        public DataSaverBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }
    }
}
