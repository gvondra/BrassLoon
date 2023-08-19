using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace BrassLoon.Authorization.CoreTest
{
    [TestClass]
    public class EmailAddressTest
    {
        [TestMethod]
        public void HashAddressTest()
        {
            byte[] hash = EmailAddress.HashAddress("test@t.est");
            Assert.IsNotNull(hash);
            Assert.AreEqual(64, hash.Length);
            Debug.WriteLine(string.Concat(hash.Select(b => b.ToString("X2", CultureInfo.InvariantCulture))));
        }
    }
}
