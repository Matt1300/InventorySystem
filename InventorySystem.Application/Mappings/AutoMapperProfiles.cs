using AutoMapper;
using InventorySystem.Domain.Entities;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.DTOs.User;

namespace InventorySystem.Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductWithPriceDto>().ReverseMap();
            CreateMap<ProductPrice, ProductPriceDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
