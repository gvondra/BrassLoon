using BrassLoon.CommonCore;
using BrassLoon.Log.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class ExceptionSaver : IExceptionSaver
    {
        public async Task Create(ISettings settings, params IException[] exceptions)
        {
            if (exceptions != null && exceptions.Length > 0)
            {
                await Saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < exceptions.Length; i += 1)
                    {
                        await exceptions[i].Create(th);
                    }
                });
            }
        }
    }
}
