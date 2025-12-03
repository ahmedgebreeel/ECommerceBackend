using AutoMapper;
using MyApp.API.DTOs.Brands;
using MyApp.API.Entities;

namespace MyApp.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Brand Mapping
            CreateMap<Brand, BrandDto>();
            CreateMap<CreateBrandDto, Brand>();
        }
    }
}
