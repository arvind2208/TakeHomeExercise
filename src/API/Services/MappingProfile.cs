using API.Models;
using API.Repositories.Entities;
using AutoMapper.Configuration;

namespace API.Services
{
    public class MappingProfile : MapperConfigurationExpression
    {
        public MappingProfile()
        {
            CreateMap<Listing, Item>()
                .ForMember(dest => dest.ListingId, m => m.MapFrom(src => src.ListingId))
                .ForMember(dest => dest.Address, m => m.MapFrom(src => $"{src.StreetNumber} {src.Street} {src.Suburb} {src.State} {src.Postcode}"))
                .ForMember(dest => dest.CategoryType, m => m.MapFrom(src => src.CategoryType.ToString()))
                .ForMember(dest => dest.StatusType, m => m.MapFrom(src => src.StatusType.ToString()))
                .ForMember(dest => dest.DisplayPrice, m => m.MapFrom(src => src.DisplayPrice))
                .ForMember(dest => dest.ShortPrice, m => m.MapFrom<ShortPriceResolver>())
                .ForMember(dest => dest.Title, m => m.MapFrom(src => src.Title));
        }
    }
}
