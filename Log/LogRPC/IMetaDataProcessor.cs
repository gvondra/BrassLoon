using Grpc.Core;

namespace LogRPC
{
    public interface IMetaDataProcessor
    {
        string GetBearerAuthorizationToken(Metadata entries);
    }
}
