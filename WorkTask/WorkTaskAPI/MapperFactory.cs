using AutoMapper;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.Framework;

namespace WorkTaskAPI
{
    public class MapperFactory
    {
        private static readonly MapperConfiguration _configuration;

        static MapperFactory()
        {
            _configuration = new MapperConfiguration(LoadConfiguration);
        }

        private static void LoadConfiguration(IMapperConfigurationExpression config)
        {
            config.CreateMap<WorkGroup, IWorkGroup>();
            config.CreateMap<IWorkGroup, WorkGroup>();
            config.CreateMap<IWorkTaskStatus, WorkTaskStatus>();
            config.CreateMap<WorkTaskStatus, IWorkTaskStatus>()
                .ForMember(s => s.IsDefaultStatus, exp => exp.MapFrom(s => s.IsDefaultStatus ?? false))
                ;
            config.CreateMap<IWorkTaskType, WorkTaskType>();
            config.CreateMap<WorkTaskType, IWorkTaskType>();
        }

        public virtual IMapper Create() => new Mapper(_configuration);
    }
}
