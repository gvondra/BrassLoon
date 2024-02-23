using BrassLoon.DataClient;

namespace BrassLoon.Authorization.Data
{
    public abstract class DataFactoryBase<T>
        where T : new()
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Fields should be private
        protected readonly IDbProviderFactory _providerFactory;
        protected readonly GenericDataFactory<T> _genericDataFactory = new GenericDataFactory<T>();
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CA1051 // Do not declare visible instance fields

        protected DataFactoryBase(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        protected T Create() => new T();
    }
}
