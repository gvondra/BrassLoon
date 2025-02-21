using BrassLoon.Authorization.Data;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using System;
using System.Globalization;
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

        public bool IsNew { get; init; }

        public Task Create(CommonCore.ISaveSettings settings) => _dataSaver.Create(settings, _data);

        public static byte[] HashAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            address = HashPrefix + address.Trim().ToLower(CultureInfo.InvariantCulture);
            return SHA512.HashData(Encoding.UTF8.GetBytes(address));
        }
    }
}
