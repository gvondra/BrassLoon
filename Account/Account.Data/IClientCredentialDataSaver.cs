﻿using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientCredentialDataSaver
    {
        Task Create(ISaveSettings settings, ClientCredentialData clientCredentialData);
    }
}
