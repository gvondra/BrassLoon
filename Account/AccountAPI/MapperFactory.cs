using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;
using System;

namespace AccountAPI
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
            config.CreateMap<Account, IAccount>();
            config.CreateMap<IAccount, Account>();
            config.CreateMap<Client, IClient>();
            config.CreateMap<IClient, Client>();
            config.CreateMap<Domain, IDomain>();
            config.CreateMap<IDomain, Domain>();
            config.CreateMap<IDomain, AccountDomain>();
            config.CreateMap<IUser, User>();
            config.CreateMap<UserInvitation, IUserInvitation>()
            .ForMember(ui => ui.ExpirationTimestamp, options => options.MapFrom<DateTime>(ui => (ui.ExpirationTimestamp ?? default).ToUniversalTime()))
            ;
            config.CreateMap<IUserInvitation, UserInvitation>();
        }

        public virtual IMapper Create() => new Mapper(_configuration);
    }
}
