using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using Polly;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class DomainService : IDomainService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public DomainService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<Domain> Get(ISettings settings, Guid id)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get) 
                .AddPath("Domain/{id}")
                .AddPathParameter("id", id.ToString())
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<Domain> response = await Policy
                .HandleResult<IResponse<Domain>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Send<Domain>(request))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<AccountDomain> GetAccountDomain(ISettings settings, Guid id)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("AccountDomain/{id}")
                .AddPathParameter("id", id.ToString())
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<AccountDomain> response = await Policy
                .HandleResult<IResponse<AccountDomain>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Send<AccountDomain>(request))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
        }
    }
}
