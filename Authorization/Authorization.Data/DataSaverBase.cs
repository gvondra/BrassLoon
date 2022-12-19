using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
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
