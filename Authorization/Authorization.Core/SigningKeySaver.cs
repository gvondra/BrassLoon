using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class SigningKeySaver : ISigningKeySaver
    {
        private readonly CommonCore.Saver _saver;

        public SigningKeySaver(CommonCore.Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, ISigningKey signingKey) => _saver.Save(new CommonCore.TransactionHandler(settings), th => signingKey.Create(th, settings));

        public Task Update(ISettings settings, ISigningKey signingKey) => _saver.Save(new CommonCore.TransactionHandler(settings), signingKey.Update);
    }
}
