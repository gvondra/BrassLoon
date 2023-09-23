using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IMetricSaver
    {
        Task Create(ISettings settings, params IMetric[] metrics);
    }
}
