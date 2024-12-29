﻿using BrassLoon.Log.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IExceptionDataSaver
    {
        Task Create(CommonData.ISaveSettings settings, ExceptionData exceptionData);
    }
}
