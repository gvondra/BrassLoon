using BrassLoon.Interface.Account.Models;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class DomainService : IDomainService
    {
        private readonly RestUtil _restUtil;

        public DomainService(RestUtil restUtil)
        {
            _restUtil = restUtil;
        }

        public async Task<Domain> Get(ISettings settings, Guid id)
        {
            RestRequest request = new RestRequest("Domain/{id}", Method.GET, DataFormat.Json);
            request.AddParameter("id", id.ToString(), ParameterType.UrlSegment);
            return await _restUtil.Execute<Domain>(settings, request);
        }
    }
}
