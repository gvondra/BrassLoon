using AutoMapper;
using BrassLoon.Config.Framework;
using BrassLoon.Interface.Config.Models;

namespace ConfigAPI
{
    public class MapperConfigurationFactory
    {
        private static readonly MapperConfiguration _mapperConfiguratin;

        static MapperConfigurationFactory()
        {
            _mapperConfiguratin = new MapperConfiguration(config =>
            {
                config.CreateMap<Item, IItem>();
                config.CreateMap<IItem, Item>();
                config.CreateMap<IItemHistory, ItemHistory>();
                config.CreateMap<Lookup, ILookup>();
                config.CreateMap<ILookup, Lookup>();
                config.CreateMap<ILookupHistory, LookupHistory>();
            });
        }

        public static Mapper CreateMapper() => new Mapper(_mapperConfiguratin);
    }
}
