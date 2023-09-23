using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IPurgeWorkerFactory
    {
        Task<Guid?> Claim(ISettings settings);
    }
}
