using AutoMapper;
using InventorySystem.Domain.Entities;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.DTOs.User;
using InventorySystem.Application.DTOs.Batch;

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
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<ProductPrice, ProductPriceDto>().ReverseMap();
            CreateMap<ProductPrice, CreateBatchDto>().ReverseMap();
            CreateMap<ProductPrice, BatchDto>().ReverseMap();
            CreateMap<ProductPrice, BatchDto>().ReverseMap();
            CreateMap<ProductPrice, BatchDto>()
                .ForMember(dest => dest.NameProduct, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<User, NewUserDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
