using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkGroupSaver : IWorkGroupSaver
    {
        private readonly Saver _saver;

        public WorkGroupSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, params IWorkGroup[] workGroups)
        {
            if (workGroups == null)
                throw new ArgumentNullException(nameof(workGroups));
            return _saver.Save(new TransactionHandler(settings), async th =>
            {
                for (int i = 0; i < workGroups.Length; i+=1) 
                {
                    await workGroups[i].Create(th);
                }
            });
        }

        public Task Update(ISettings settings, params IWorkGroup[] workGroups)
        {
            if (workGroups == null)
                throw new ArgumentNullException(nameof(workGroups));
            return _saver.Save(new TransactionHandler(settings), async th =>
            {
                for (int i = 0; i < workGroups.Length; i += 1)
                {
                    await workGroups[i].Update(th);
                }
            });
        }
    }
}
