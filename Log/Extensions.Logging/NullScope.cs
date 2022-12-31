using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal sealed class NullScope : IDisposable
    {        
        private NullScope() { }

        public static NullScope Instance { get; } = new NullScope();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
