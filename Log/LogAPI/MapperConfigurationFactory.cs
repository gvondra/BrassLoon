using AutoMapper;
using BrassLoon.Interface.Log.Models;
using BrassLoon.Log.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI
{
    public class MapperConfigurationFactory
    {
        private static MapperConfiguration _mapperConfiguratin;

        static MapperConfigurationFactory()
        {
            _mapperConfiguratin = new MapperConfiguration(config =>
            {
                config.CreateMap<Trace, ITrace>();
                config.CreateMap<ITrace, Trace>();
            });
        }

        public static Mapper CreateMapper()
        {
            return new Mapper(_mapperConfiguratin);
        }
    }
}
