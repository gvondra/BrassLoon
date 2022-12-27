using AutoMapper;
using BrassLoon.Interface.Log.Models;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;
using System;
using System.Collections.Generic;

namespace LogAPI
{
    public class MapperFactory
    {
        private static MapperConfiguration _mapperConfiguratin;

        static MapperFactory()
        {
            _mapperConfiguratin = new MapperConfiguration(LoadConfiguration);
        }

        private static void LoadConfiguration(IMapperConfigurationExpression config)
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
        }

        public Mapper Create() => new Mapper(_mapperConfiguratin);
    }
}
