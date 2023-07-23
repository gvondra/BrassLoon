using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class EmailAddress : IEmailAddress
    {
        private const string HashPrefix = "brass-loon-authorization";

        private readonly EmailAddressData _data;
        private readonly IEmailAddressDataSaver _dataSaver;

        public EmailAddress(EmailAddressData data, IEmailAddressDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid EmailAddressId => _data.EmailAddressId;

        public string Address => _data.Address;

        public byte[] AddressHash => _data.AddressHash;

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public bool IsNew => _data.Manager.GetState(_data) == DataClient.DataState.New;

        public Task Create(CommonCore.ITransactionHandler transactionHandler)
        {
            return _dataSaver.Create(transactionHandler, _data);
        }

        public static byte[] HashAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            address = HashPrefix + address.Trim().ToLower();
            SHA512 alogrithm = SHA512.Create();
            return alogrithm.ComputeHash(Encoding.UTF8.GetBytes(address));
        }
    }
}
