using BrassLoon.Authorization.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class ClientSaver : IClientSaver
    {
        private readonly CommonCore.Saver _saver;

        public ClientSaver(CommonCore.Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, IClient client)
        {
            return _saver.Save(new CommonCore.TransactionHandler(settings), th => client.Create(th, settings));
        }

        public Task Update(ISettings settings, IClient client)
        {
            return _saver.Save(new CommonCore.TransactionHandler(settings), th => client.Update(th, settings));
        }
    }
}
