using AutoMapper;
using InventorySystem.Application.Constants;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Interfaces;
using InventorySystem.Application.Interfaces.UnitOfWork;
using InventorySystem.Application.Utilities;
using InventorySystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;



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

        public async Task<ApiResponse<IEnumerable<ProductQuantityDto>>> GetAll()
        {
            try
            {
                var products = await _unitOfWork.Repository<Product>().GetAllAsync();

                var quantities = await GetQuantityById();
                var productsDto = _mapper.Map<IEnumerable<ProductQuantityDto>>(products);

                foreach (var product in productsDto)
                {
                    if (quantities.TryGetValue(product.Id, out var quantity))
                    {
                        product.Quantity = quantity;
                    }
                    else
                    {
                        product.Quantity = 0;
                    }
                }

                return new ApiResponse<IEnumerable<ProductQuantityDto>>(true, productsDto, Messages.ProductsRetrieved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos: {Message}", ex.Message);
                return new ApiResponse<IEnumerable<ProductQuantityDto>>(false, null!, Messages.ErrorOccurred);
            }
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



        public async Task<ApiResponse<ProductDto>> AddProduct(AddProductDto product)
        {
            if (product is null || string.IsNullOrEmpty(product.Name) || string.IsNullOrEmpty(product.Description))
                return new ApiResponse<ProductDto>(false, null!, Messages.EmptyRequest);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newProduct = _mapper.Map<Product>(product);
                await _unitOfWork.Repository<Product>().AddAsync(newProduct);
                await _unitOfWork.SaveChangesAsync();

                var productDto = _mapper.Map<ProductDto>(newProduct);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<ProductDto>(true, productDto, Messages.ProductAdded);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error al agregar un nuevo producto: {Message}", ex.Message);
                return new ApiResponse<ProductDto>(false, null!, Messages.ErrorOccurred);
            }
        }


        public async Task<ApiResponse<ProductDto>> UpdateProduct(int id, AddProductDto product)
        {
            if (product is null)
                return new ApiResponse<ProductDto>(false, null!, Messages.EmptyRequest);

            try
            {
                var existingProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

                if (existingProduct == null)
                    return new ApiResponse<ProductDto>(false, null!, Messages.ProductNotFound);

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;

                _unitOfWork.Repository<Product>().Update(existingProduct);
                await _unitOfWork.SaveChangesAsync();

                var productDto = _mapper.Map<ProductDto>(existingProduct);

                return new ApiResponse<ProductDto>(true, productDto, Messages.ProductUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto: {Message}", ex.Message);
                return new ApiResponse<ProductDto>(false, null!, Messages.ErrorOccurred);
            }
        }


        public async Task<ApiResponse<bool>> DeleteProduct(int id)
        {
            if (id <= 0)
                return new ApiResponse<bool>(false, false, Messages.EmptyRequest);

            try
            {
                var findProduct = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(p => p.Id == id);
                if (findProduct == null)
                    return new ApiResponse<bool>(false, false, Messages.ProductNotFound);

                var productPrices = await _unitOfWork.Repository<ProductPrice>()
                    .GetQueryable()
                    .Where(pp => pp.ProductId == id)
                    .ToListAsync();

                _unitOfWork.Repository<Product>().Delete(findProduct);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<bool>(true, true, Messages.ProductDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto: {Message}", ex.Message);
                return new ApiResponse<bool>(false, false, Messages.ErrorOccurred);
            }
        }



        private async Task<Dictionary<int, int>> GetQuantityById()
        {
            var productPrices = await _unitOfWork.Repository<ProductPrice>().GetAllAsync();
            var quantities = productPrices
                .GroupBy(pp => pp.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(pp => pp.Quantity));

            return quantities;
        }
    }
}
