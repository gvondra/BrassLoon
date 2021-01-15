using AutoMapper;
using BrassLoon.Interface.Config.Models;
using BrassLoon.Config.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigAPI
{
    public class MapperConfigurationFactory
    {
        private static MapperConfiguration _mapperConfiguratin;

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

        public static Mapper CreateMapper()
        {
            return new Mapper(_mapperConfiguratin);
        }
    }
}
