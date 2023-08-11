using BrassLoon.Interface.Account;
using Grpc.Core;
using LogRPC.Protos;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace LogRPC.Services
{
    public class TokenService : Protos.TokenService.TokenServiceBase
    {
        private readonly IOptions<Settings> _settings;
        private readonly SettingsFactory _settingsFactory;
        private readonly ITokenService _tokenService;

        public TokenService(
            IOptions<Settings> settings,
            SettingsFactory settingsFactory,
            ITokenService tokenService)
        {
            _settings = settings;
            _settingsFactory = settingsFactory;
            _tokenService = tokenService;
        }

        public override async Task<Token> Create(TokenRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.ClientId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing {nameof(request.ClientId)} parameter value");
            if (string.IsNullOrEmpty(request.Secret))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing {nameof(request.Secret)} parameter value");
            Guid clientId = Guid.Parse(request.ClientId);
            if (clientId.Equals(Guid.Empty))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad Request"), $"Missing {nameof(request.ClientId)} parameter value");
            string token = await _tokenService.CreateClientCredentialToken(_settingsFactory.CreateAccount(_settings.Value), Guid.Parse(request.ClientId), request.Secret);
            return new Token { Value = token };
        }
    }
}
