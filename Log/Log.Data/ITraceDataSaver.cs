using BrassLoon.Log.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface ITraceDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, TraceData traceData);
    }
}
