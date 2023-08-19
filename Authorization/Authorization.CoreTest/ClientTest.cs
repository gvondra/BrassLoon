using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using Moq;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.CoreTest
{
    [TestClass]
    public class ClientTest
    {
        [TestMethod]
        public async Task AuthenticateSecretTest()
        {
            Guid secretKey = Guid.NewGuid();
            string actualSecret = "test secret";
            ClientData data = new ClientData()
            {
                SecretKey = secretKey,
                SecretSalt = Client.CreateSalt(),
                IsActive = true
            };
            Mock<ISettings> settings = new Mock<ISettings>();
            Mock<IClientDataSaver> dataSaver = new Mock<IClientDataSaver>();
            Mock<IRoleFactory> roleFactory = new Mock<IRoleFactory>();
            Mock<IRoleDataSaver> roleDataSaver = new Mock<IRoleDataSaver>();
            KeyVaultFake keyVault = new KeyVaultFake();
            await keyVault.SetSecret(settings.Object, secretKey.ToString("D"), Convert.ToBase64String(Client.HashSecret(actualSecret, data.SecretSalt)));
            Client client = new Client(data, dataSaver.Object, keyVault, roleFactory.Object, roleDataSaver.Object, null);
            Assert.IsTrue(await client.AuthenticateSecret(settings.Object, actualSecret));
            Assert.IsFalse(await client.AuthenticateSecret(settings.Object, actualSecret.ToUpper(CultureInfo.InvariantCulture)));
            client.IsActive = false;
            Assert.IsFalse(await client.AuthenticateSecret(settings.Object, actualSecret));
        }
    }
}
