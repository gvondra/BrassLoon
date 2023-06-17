using BrassLoon.WorkTask.Core;
using System;

namespace WorkTask.Core.Test
{
    [TestClass]
    public class WorkTaskContextHashTest
    {
        [TestMethod]
        [DataRow("test")]
        [DataRow("")]
        public void ComputeTest(string value)
        {
            byte[] result = WorkTaskContextHash.Compute(value);
            Assert.IsNotNull(result);
            Assert.AreEqual(64, result.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComputeNullTest()
        {
            WorkTaskContextHash.Compute(null);
            Assert.Fail();
        }
    }
}
