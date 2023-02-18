using BrassLoon.DataClient;

namespace BrassLoon.WorkTask.Data.Internal
{
    public abstract class DataFactoryBase<T> where T : new()
    {
        protected readonly IDbProviderFactory _providerFactory;
        protected readonly GenericDataFactory<T> _genericDataFactory = new GenericDataFactory<T>();

        public DataFactoryBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        protected T Create() => new T();
    }
}
