﻿using BrassLoon.Account.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserDataSaver
    {
        Task Create(ISaveSettings settings, UserData userData);
        Task Update(ISaveSettings settings, UserData userData);
    }
}
