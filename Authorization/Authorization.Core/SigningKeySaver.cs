using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class SigningKeySaver : ISigningKeySaver
    {
        public Task Create(ISettings settings, ISigningKey signingKey)
            => CommonCore.Saver.Save(new SaveSettings(settings), signingKey.Create);

        public Task Update(ISettings settings, ISigningKey signingKey)
            => CommonCore.Saver.Save(new SaveSettings(settings), signingKey.Update);
    }
}
