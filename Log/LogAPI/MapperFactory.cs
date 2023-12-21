using AutoMapper;
using BrassLoon.Interface.Log.Models;
using BrassLoon.Log.Framework;
using BrassLoon.Log.Framework.Enumerations;

namespace LogAPI
{
    public class MapperFactory
    {
        private static readonly MapperConfiguration _mapperConfiguratin = new MapperConfiguration(LoadConfiguration);

        private static void LoadConfiguration(IMapperConfigurationExpression config)
        {
            _ = config.CreateMap<EventId, IEventId>();
            _ = config.CreateMap<IEventId, EventId>();
            _ = config.CreateMap<Exception, IException>();
            _ = config.CreateMap<IException, Exception>();
            _ = config.CreateMap<Metric, IMetric>();
            _ = config.CreateMap<IMetric, Metric>();
            _ = config.CreateMap<Trace, ITrace>();
            _ = config.CreateMap<ITrace, Trace>();
            _ = config.CreateMap<PurgeWorker, IPurgeWorker>()
            .ForMember(pw => pw.Status, options => options.MapFrom(pw => (PurgeWorkerStatus)(pw.Status ?? (short)PurgeWorkerStatus.Ready)))
            ;
            _ = config.CreateMap<IPurgeWorker, PurgeWorker>()
            .ForMember(pw => pw.Status, options => options.MapFrom(pw => (short)pw.Status))
            ;
        }

        public Mapper Create() => new Mapper(_mapperConfiguratin);
    }
}
