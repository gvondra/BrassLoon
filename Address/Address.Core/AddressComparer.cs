using BrassLoon.Address.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BrassLoon.Address.Core
{
    internal class AddressComparer : IEqualityComparer<IAddress>
    {
        public bool Equals(IAddress x, IAddress y) => x.Equals(y);
        public int GetHashCode([DisallowNull] IAddress obj) => obj.GetHashCode();
    }
}
