using BrassLoon.CommonCore;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class TraceSaver : ITraceSaver
    {
        public async Task Create(ISettings settings, params ITrace[] traces)
        {
            if (traces != null && traces.Length > 0)
            {
                Saver saver = new Saver();
                await saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < traces.Length; i += 1)
                    {
                        await traces[i].Create(th);
                    }
                });
            }            
        }
    }
}
