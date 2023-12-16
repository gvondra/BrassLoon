using AutoMapper;
using BrassLoon.Authorization.Framework;
using BrassLoon.Interface.Authorization.Models;
namespace AuthorizationAPI
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
            _ = config.CreateMap<IRole, AppliedRole>();
            _ = config.CreateMap<Client, IClient>()
                .ForMember(c => c.IsActive, exp => exp.MapFrom(c => c.IsActive ?? true))
                ;
            _ = config.CreateMap<IClient, Client>();
            _ = config.CreateMap<Role, IRole>()
                .ForMember(r => r.IsActive, exp => exp.MapFrom(r => r.IsActive ?? true))
                ;
            _ = config.CreateMap<IRole, Role>();
            _ = config.CreateMap<SigningKey, ISigningKey>()
            .ForMember(k => k.IsActive, exp => exp.MapFrom(k => k.IsActive ?? true))
                ;
            _ = config.CreateMap<ISigningKey, SigningKey>();
            _ = config.CreateMap<User, IUser>();
            _ = config.CreateMap<IUser, User>();
        }

        public virtual IMapper Create() => new Mapper(_configuration);
    }
}
