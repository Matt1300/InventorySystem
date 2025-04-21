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
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<Product, ProductWithPriceDto>().ReverseMap();
            CreateMap<Product, ProductQuantityDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, NewProductDto>().ReverseMap();
            CreateMap<User, NewUserDto>().ReverseMap();
            CreateMap<ProductPrice, ProductPriceDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<Product, UpdateProductDto>().ReverseMap();
        }
    }
}
