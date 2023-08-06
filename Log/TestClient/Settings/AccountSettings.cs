using BrassLoon.Interface.Account;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.TestClient.Settings
{
    public sealed class AccountSettings : ISettings
    {
        public string BaseAddress { get; set; }

        public Task<string> GetToken() => throw new NotImplementedException();
    }
}