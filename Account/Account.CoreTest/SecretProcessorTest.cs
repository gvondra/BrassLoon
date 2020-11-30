using BrassLoon.Account.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Account.CoreTest
{
    [TestClass]
    public class SecretProcessorTest
    {
        [TestMethod]
        public void VerifyTest()
        {
            SecretProcessor secretProcessor = new SecretProcessor();
            string secret = secretProcessor.Create();
            Assert.IsNotNull(secret);
            Assert.IsTrue(Regex.IsMatch(secret, "^[0-9A-F]{64}$", RegexOptions.IgnoreCase));
            byte[] hash = secretProcessor.Hash(secret);
            Assert.IsNotNull(hash);
            Assert.AreEqual(64, hash.Length);
            Assert.IsTrue(secretProcessor.Verify(secret, hash));
            Assert.IsFalse(secretProcessor.Verify(new string('*', 64), hash));
        }
    }
}
