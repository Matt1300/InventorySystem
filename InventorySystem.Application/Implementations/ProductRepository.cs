using AutoMapper;
using InventorySystem.Application.Constants;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Interfaces;
using InventorySystem.Application.Interfaces.UnitOfWork;
using InventorySystem.Application.Utilities;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ApiResponse<bool>> AddAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync()
        {
            try
            {
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();
                var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return new ApiResponse<IEnumerable<ProductDto>>(true, productsDto, Messages.ProductsRetrieved);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProductDto>>(false, null!, Messages.ErrorOccurred);
            }
        }

        public Task<ApiResponse<ProductDto?>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ProductDto?>> GetProductDtoWithDetailsAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
