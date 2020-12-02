using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IMetricSaver
    {
        Task Create(ISettings settings, params IMetric[] metrics);
    }
}
