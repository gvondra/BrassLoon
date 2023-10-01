using Grpc.Core;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    internal static class RpcUtil
    {
        internal static async Task<Metadata> CreateMetaDataWithAuthHeader(ISettings settings)
            => CreateMetaDataWithAuthHeader(await settings.GetToken());

        internal static Metadata CreateMetaDataWithAuthHeader(string token)
        {
            return new Metadata()
            {
                { "Authorization", string.Format(CultureInfo.InvariantCulture, "Bearer {0}", token) }
            };
        }
    }
}
