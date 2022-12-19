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
            config.CreateMap<IRole, AppliedRole>();
            config.CreateMap<Client, IClient>()
                .ForMember(c => c.IsActive, exp => exp.MapFrom(c => c.IsActive ?? true))
                ;
            config.CreateMap<IClient, Client>();
            config.CreateMap<Role, IRole>()
                .ForMember(r => r.IsActive, exp => exp.MapFrom(r => r.IsActive ?? true))    
                ;
            config.CreateMap<IRole, Role>();
            config.CreateMap<SigningKey, ISigningKey>()
            .ForMember(k => k.IsActive, exp => exp.MapFrom(k => k.IsActive ?? true))
                ;
            config.CreateMap<ISigningKey, SigningKey>();
            config.CreateMap<User, IUser>();
            config.CreateMap<IUser, User>();
        }

        public virtual IMapper Create() => new Mapper(_configuration);
    }
}
