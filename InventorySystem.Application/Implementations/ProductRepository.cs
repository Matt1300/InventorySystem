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

        public async Task<ApiResponse<ProductWithPriceDto?>> GetProductDtoWithDetailsAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(
                    p => p.Id == productId,
                    p => p.Prices
                );

                if (product == null)
                    return new ApiResponse<ProductWithPriceDto?>(false, null!, Messages.ProductNotFound);

                var dto = new ProductWithPriceDto
                {
                    Id = productId,
                    Name = product.Name,
                    Description = product.Description!,
                    Prices = product.Prices.Select(price => new ProductPriceDto
                    {
                        Id = price.Id,
                        ProductId = price.ProductId,
                        Price = price.Price,
                        BatchNumber = price.BatchNumber,
                        EntryDate = price.EntryDate
                    }).ToList()
                };

                return new ApiResponse<ProductWithPriceDto?>(true, dto, Messages.ProductsRetrieved);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductWithPriceDto?>(false, null!, Messages.ErrorOccurred);
            }
        }

        public Task<ApiResponse<bool>> UpdateAsync(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
