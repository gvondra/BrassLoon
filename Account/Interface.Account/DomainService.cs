using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class DomainService : IDomainService
    {
        private readonly IService _service;

        public DomainService(IService service)
        {
            _service = service;
        }

        public async Task<Domain> Get(ISettings settings, Guid id)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get) 
                .AddPath("Domain/{id}")
                .AddPathParameter("id", id.ToString())
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return (await _service.Send<Domain>(request)).Value;
        }
    }
}
