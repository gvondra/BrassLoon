using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public interface IException
    {
        long ExceptionId { get; }
        Guid DomainId { get; }
        string Message { get; set; }
        string TypeName { get; set; }
        string Source { get; set; }
        string AppDomain { get; set; }
        string TargetSite { get; set; }
        string StackTrace { get; set; }
        dynamic Data { get; set; }
        DateTime CreateTimestamp { get; }
        string Category { get; set; }
        string Level { get; set; }

        Task<IException> GetInnerException(ISettings settings);
        Task Create(ISaveSettings settings);
    }
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
}
