using Grpc.Net.Client;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal interface IAccessTokenFactory
    {
        Task<string> GetAccessToken(LoggerConfiguration loggerConfiguration, GrpcChannel channel);
    }
}
