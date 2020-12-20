using AutoMapper;
using BrassLoon.Interface.Log.Models;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using System;
using System.Collections.Generic;

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
                config.CreateMap<PurgeWorker, IPurgeWorker>()
                .ForMember(pw => pw.Status, options => options.MapFrom(pw => (PurgeWorkerStatus)(pw.Status ?? (short)PurgeWorkerStatus.Ready)))
                ;
                config.CreateMap<IPurgeWorker, PurgeWorker>()
                .ForMember(pw => pw.Status, options => options.MapFrom(pw => (short)pw.Status))
                ;
            });
        }

        public static Mapper CreateMapper()
        {
            return new Mapper(_mapperConfiguratin);
        }
    }
}
