using AutoMapper;
using InventorySystem.Application.Constants;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Interfaces;
using InventorySystem.Application.Interfaces.UnitOfWork;
using InventorySystem.Application.Utilities;
using InventorySystem.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InventorySystem.Application.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<ApiResponse<bool>> AddProduct(ProductDto product)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAll()
        {
            try
            {
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();
                var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
                return new ApiResponse<IEnumerable<ProductDto>>(true, productsDto, Messages.ProductsRetrieved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos: {Message}", ex.Message);
                return new ApiResponse<IEnumerable<ProductDto>>(false, null!, Messages.ErrorOccurred);
            }
        }

        public Task<ApiResponse<ProductDto?>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<ProductWithPriceDto?>> GetProductWithDetails(int productId)
        {
            try     
            {
                var product = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(
                    p => p.Id == productId,
                    p => p.Prices
                );

                if (product == null)
                    return new ApiResponse<ProductWithPriceDto?>(false, null!, Messages.ProductNotFound);

                var dto = _mapper.Map<ProductWithPriceDto>(product);

                return new ApiResponse<ProductWithPriceDto?>(true, dto, Messages.ProductsRetrieved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el producto con id {id}: {Message}", productId, ex.Message);
                return new ApiResponse<ProductWithPriceDto?>(false, null!, Messages.ErrorOccurred);
            }
        }

        public Task<ApiResponse<bool>> UpdateProduct(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
