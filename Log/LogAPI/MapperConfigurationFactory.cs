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
                config.CreateMap<BrassLoon.Interface.Log.Models.Exception, IException>();
                config.CreateMap<IException, BrassLoon.Interface.Log.Models.Exception>();
                config.CreateMap<Metric, IMetric>();
                config.CreateMap<IMetric, Metric>();
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
