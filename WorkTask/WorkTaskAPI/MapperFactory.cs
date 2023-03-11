using AutoMapper;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.Framework;
using System.Collections.Generic;

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
            config.CreateMap<IReadOnlyList<string>, List<string>>()
                .ConvertUsing(rol => new List<string>(rol));
            config.CreateMap<WorkGroup, IWorkGroup>()
                .ForMember(g => g.MemberUserIds, exp => exp.Ignore());
            config.CreateMap<IWorkGroup, WorkGroup>();
            config.CreateMap<IWorkTask, WorkTask>();
            config.CreateMap<WorkTask, IWorkTask>();
            config.CreateMap<IComment, Comment>();
            config.CreateMap<IWorkTaskContext, WorkTaskContext>();
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
