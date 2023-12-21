using System;

namespace BrassLoon.Extensions.Logging
{
    internal sealed class NullScope : IDisposable
    {
        private NullScope() { }

        public static NullScope Instance { get; } = new NullScope();

        public void Dispose() { }
    }
}
