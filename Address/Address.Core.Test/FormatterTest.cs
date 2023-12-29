namespace BrassLoon.Address.Core.Tet
{
    [TestClass]
    public class FormatterTest
    {
        [TestMethod]
        public void UnformatPostalCodeTest()
        {
            Assert.AreEqual("53554", Formatter.UnformatPostalCode("53554"));
            Assert.AreEqual("20500", Formatter.UnformatPostalCode("20500"));
            Assert.AreEqual("555554444", Formatter.UnformatPostalCode(" 55555-4444 "));
            Assert.AreEqual("aAzZ", Formatter.UnformatPostalCode("aAzZ"));
        }
    }
}
