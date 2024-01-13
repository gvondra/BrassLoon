using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;

namespace AccountAPI
{
    public class MapperFactory
    {
        private static readonly MapperConfiguration _configuration = new MapperConfiguration(LoadConfiguration);

        private static void LoadConfiguration(IMapperConfigurationExpression config)
        {
            _ = config.CreateMap<Account, IAccount>();
            _ = config.CreateMap<IAccount, Account>();
            _ = config.CreateMap<Client, IClient>();
            _ = config.CreateMap<IClient, Client>();
            _ = config.CreateMap<Domain, IDomain>();
            _ = config.CreateMap<IDomain, Domain>();
            _ = config.CreateMap<IDomain, AccountDomain>();
            _ = config.CreateMap<IUser, User>();
            _ = config.CreateMap<UserInvitation, IUserInvitation>()
            .ForMember(ui => ui.ExpirationTimestamp, options => options.MapFrom(ui => (ui.ExpirationTimestamp ?? default).ToUniversalTime()))
            ;
            _ = config.CreateMap<IUserInvitation, UserInvitation>();
        }

        public virtual IMapper Create() => new Mapper(_configuration);
    }
}
