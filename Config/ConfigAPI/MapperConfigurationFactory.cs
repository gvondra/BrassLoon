using AutoMapper;
using BrassLoon.Config.Framework;
using BrassLoon.Interface.Config.Models;

namespace ConfigAPI
{
    public static class MapperConfigurationFactory
    {
        private static readonly MapperConfiguration _mapperConfiguratin = CreateMapperConfiguration();

        private static MapperConfiguration CreateMapperConfiguration()
        {
            return new MapperConfiguration(config =>
            {
                _ = config.CreateMap<Item, IItem>();
                _ = config.CreateMap<IItem, Item>();
                _ = config.CreateMap<IItemHistory, ItemHistory>();
                _ = config.CreateMap<Lookup, ILookup>();
                _ = config.CreateMap<ILookup, Lookup>();
                _ = config.CreateMap<ILookupHistory, LookupHistory>();
            });
        }

        public static Mapper CreateMapper() => new Mapper(_mapperConfiguratin);
    }
}
