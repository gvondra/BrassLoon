using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Internal.SqlClient
{
    public abstract class DataFactoryBase<T>
        where T : new()
    {
        private readonly IDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<T> _genericDataFactory = new GenericDataFactory<T>();

        protected DataFactoryBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        protected IDbProviderFactory ProviderFactory => _providerFactory;
        protected GenericDataFactory<T> GenericDataFactory => _genericDataFactory;
        protected T Create() => new T();
    }
}
