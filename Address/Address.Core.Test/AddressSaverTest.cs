using Azure.Security.KeyVault.Secrets;
using BrassLoon.Address.Data;
using BrassLoon.Address.Data.Models;
using BrassLoon.Address.Framework;
using BrassLoon.CommonCore;
using BrassLoon.DataClient;
using System.Collections.Generic;

namespace BrassLoon.Address.Core.Tet
{
    [TestClass]
    public class AddressSaverTest
    {
        private const string KEY_VAULT_ADDRESS = "urn:test:address";
        private static readonly Guid DOMAIN_ID = Guid.NewGuid();

        [TestMethod]
        public async Task SaveTest()
        {
            Dictionary<string, string> encryptionKeys = new Dictionary<string, string>();

            Mock<Framework.ISettings> settings = new Mock<Framework.ISettings>();
            settings.SetupGet(s => s.KeyVaultAddress).Returns(KEY_VAULT_ADDRESS);

            Mock<IKeyVault> keyVault = new Mock<IKeyVault>();
            keyVault.Setup(kv => kv.SetSecret(KEY_VAULT_ADDRESS, It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Returns((string url, string id, string value) =>
                {
                    encryptionKeys[id] = value;
                    return Task.FromResult(new KeyVaultSecret(id, value));
                });
            keyVault.Setup(kv => kv.GetSecret(KEY_VAULT_ADDRESS, It.IsNotNull<string>()))
                .Returns((string url, string id) =>
                {
                    return Task.FromResult(
                        new KeyVaultSecret(id, encryptionKeys[id]));
                });

            Mock<IAddress> address = new Mock<IAddress>();
            address.SetupAllProperties();
            address.SetupGet(a => a.DomainId).Returns(DOMAIN_ID);
            address.Object.Attention = "Attention Joe";
            address.Object.Addressee = "White House";
            address.Object.Delivery = "1600 Pensylvania Ave";
            address.Object.City = "Washington";
            address.Object.Territory = "DC";
            address.Object.PostalCode = "20500";
            address.Object.Country = string.Empty;

            Mock<IAddressDataFactory> dataFactory = new Mock<IAddressDataFactory>();

            Mock<IAddressDataSaver> dataSaver = new Mock<IAddressDataSaver>();
            dataSaver.Setup(ds => ds.Create(It.IsAny<CommonData.ISaveSettings>(), It.IsNotNull<AddressData>()))
                .Returns((ISqlTransactionHandler th, AddressData d) =>
                {
                    d.AddressId = Guid.NewGuid();
                    d.CreateTimestamp = DateTime.UtcNow;
                    return Task.CompletedTask;
                });

            AddressFactory addressFactory = new AddressFactory(dataFactory.Object, keyVault.Object);
            AddressSaver addressSaver = new AddressSaver(addressFactory, dataSaver.Object, keyVault.Object);

            IAddress result = await addressSaver.Save(settings.Object, address.Object);
            Assert.IsNotNull(result);
            Assert.AreNotSame(address.Object, result);

            Assert.AreEqual(DOMAIN_ID, result.DomainId);
            Assert.AreEqual(address.Object.Attention, result.Attention);
            Assert.AreEqual(address.Object.Addressee, result.Addressee);
            Assert.AreEqual(address.Object.Delivery, result.Delivery);
            Assert.AreEqual(address.Object.City, result.City);
            Assert.AreEqual(address.Object.Territory, result.Territory);
            Assert.AreEqual(address.Object.PostalCode, result.PostalCode);
            Assert.AreEqual(address.Object.Country, result.Country);
            Assert.AreEqual(string.Empty, result.County);
            Assert.AreNotEqual(Guid.Empty, result.AddressId);
            Assert.AreNotEqual(DateTime.MinValue, result.CreateTimestamp);
            Assert.IsTrue(DateTime.UtcNow.AddMinutes(-2) <= result.CreateTimestamp && result.CreateTimestamp <= DateTime.UtcNow);

            keyVault.Verify(kv => kv.SetSecret(KEY_VAULT_ADDRESS, It.IsNotNull<string>(), It.IsNotNull<string>()), Times.Once);
            dataSaver.Verify(ds => ds.Create(It.IsNotNull<CommonData.ISaveSettings>(), It.IsNotNull<AddressData>()), Times.Once);
        }
    }
}
