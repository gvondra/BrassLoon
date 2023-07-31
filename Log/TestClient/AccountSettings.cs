using BrassLoon.Interface.Account;
using System;
using System.Threading.Tasks;

namespace TestClient 
{
    public sealed class AccountSettings : ISettings
    {
        private readonly ITokenService _tokenService;
        private string _token = null;

        public AccountSettings(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public string BaseAddress { get; set; }
        public Guid AccountClientId { get; set; }
        public string AccountClientSecrect { get; set; }

        public async Task<string> GetToken()
        {
            if (_token == null)
                _token = await _tokenService.CreateClientCredentialToken(this, AccountClientId, AccountClientSecrect);
            return _token;
        }
    }
}