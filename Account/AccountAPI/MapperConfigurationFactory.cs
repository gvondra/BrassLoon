using AutoMapper;
using BrassLoon.Account.Framework;
using BrassLoon.Interface.Account.Models;

namespace AccountAPI
{
    public class MapperConfigurationFactory
    {
        private static MapperConfiguration _mapperConfiguratin;

        static MapperConfigurationFactory()
        {
            _mapperConfiguratin = new MapperConfiguration(config =>
            {
                config.CreateMap<Account, IAccount>();
                config.CreateMap<IAccount, Account>();
                config.CreateMap<Client, IClient>();
                config.CreateMap<IClient, Client>();
                config.CreateMap<Domain, IDomain>();
                config.CreateMap<IDomain, Domain>();
                config.CreateMap<UserInvitation, IUserInvitation>();
                config.CreateMap<IUserInvitation, UserInvitation>();
            });
        }

        public static Mapper CreateMapper()
        {
            return new Mapper(_mapperConfiguratin);
        }
    }
}
