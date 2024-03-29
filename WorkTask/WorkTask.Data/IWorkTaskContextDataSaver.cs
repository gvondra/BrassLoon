﻿using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskContextDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkTaskContextData data);
    }
}
