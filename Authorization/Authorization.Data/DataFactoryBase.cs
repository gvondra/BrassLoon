using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
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
