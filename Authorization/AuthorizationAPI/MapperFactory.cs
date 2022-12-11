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
            config.CreateMap<Role, IRole>();
            config.CreateMap<IRole, Role>();
        }

        public virtual IMapper Create() => new Mapper(_configuration);
    }
}
