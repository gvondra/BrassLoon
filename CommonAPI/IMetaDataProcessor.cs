using Grpc.Core;

namespace BrassLoon.CommonAPI
{
    public interface IMetaDataProcessor
    {
        string GetBearerAuthorizationToken(Metadata entries);
    }
}
