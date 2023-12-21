using AutoMapper;
using BrassLoon.Interface.WorkTask.Models;
using BrassLoon.WorkTask.Framework;
using System.Collections.Generic;

namespace WorkTaskAPI
{
    public class MapperFactory
    {
        private static readonly MapperConfiguration _configuration = new MapperConfiguration(LoadConfiguration);

        private static void LoadConfiguration(IMapperConfigurationExpression config)
        {
            config.CreateMap<IReadOnlyList<string>, List<string>>()
                .ConvertUsing(rol => new List<string>(rol));
            _ = config.CreateMap<WorkGroup, IWorkGroup>()
                .ForMember(g => g.MemberUserIds, exp => exp.Ignore())
                .ForMember(g => g.WorkTaskTypeIds, exp => exp.Ignore());
            _ = config.CreateMap<IWorkGroup, WorkGroup>();
            _ = config.CreateMap<IWorkTask, WorkTask>();
            _ = config.CreateMap<WorkTask, IWorkTask>()
                .ForMember(wt => wt.WorkTaskContexts, exp => exp.Ignore());
            _ = config.CreateMap<IComment, Comment>();
            _ = config.CreateMap<IWorkTaskContext, WorkTaskContext>();
            _ = config.CreateMap<IWorkTaskStatus, WorkTaskStatus>();
            _ = config.CreateMap<WorkTaskStatus, IWorkTaskStatus>()
                .ForMember(s => s.IsDefaultStatus, exp => exp.MapFrom(s => s.IsDefaultStatus ?? false));
            _ = config.CreateMap<IWorkTaskType, WorkTaskType>();
            _ = config.CreateMap<WorkTaskType, IWorkTaskType>();
        }

        public virtual IMapper Create() => new Mapper(_configuration);
    }
}
