﻿using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IMetricDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, MetricData metricData);
    }
}
