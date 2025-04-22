using AutoMapper;
using InventorySystem.Application.Constants;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Interfaces;
using InventorySystem.Application.Interfaces.UnitOfWork;
using InventorySystem.Application.Utilities;
using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace InventorySystem.Application.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepository> _logger;
        private const string PRODUCT_PREFIX = "PRD";

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

                var quantities = await GetQuantities();
                var productsDto = _mapper.Map<IEnumerable<ProductQuantityDto>>(products);

                foreach (var product in productsDto)
                {
                    product.ActualPrice = await GetActualPrice(product.Id);
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
                var quantity = await GetQuantityById(productId);
                
                var product = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(
                    p => p.Id == productId,
                    p => p.Prices
                );
                
                if (product == null)
                    return new ApiResponse<ProductWithPriceDto?>(false, null!, Messages.ProductNotFound);

                var dto = _mapper.Map<ProductWithPriceDto>(product);
                dto.Quantity = quantity;
                dto.Code = product.Code;
                dto.ActualPrice = await GetActualPrice(productId);
                return new ApiResponse<ProductWithPriceDto?>(true, dto, Messages.ProductsRetrieved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el producto con id {id}: {Message}", productId, ex.Message);
                return new ApiResponse<ProductWithPriceDto?>(false, null!, Messages.ErrorOccurred);
            }
        }


        public async Task<ApiResponse<IEnumerable<SearchProductDto>>> SearchProduct(string term)
        {
            if (string.IsNullOrEmpty(term))
                return new ApiResponse<IEnumerable<SearchProductDto>>(false, null!, Messages.EmptyRequest);

            try
            {
                var products = await _unitOfWork.Repository<Product>()
                    .GetQueryable()
                    .Where(p => p.Name.ToLower().Contains(term.ToLower()) && p.IsActive)
                    .OrderBy(p => p.Name)
                    .Take(10)
                    .Select(p => new SearchProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                    })
                    .ToListAsync();

                if (products == null || !products.Any())
                    return new ApiResponse<IEnumerable<SearchProductDto>>(false, null!, Messages.ProductNotFound);

                return new ApiResponse<IEnumerable<SearchProductDto>>(true, products, Messages.ProductsRetrieved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar el producto: {Message}", ex.Message);
                return new ApiResponse<IEnumerable<SearchProductDto>>(false, null!, Messages.ErrorOccurred);
            }
        }


        public async Task<ApiResponse<ProductDto>> AddProduct(NewProductDto product)
        {
            if (product is null || string.IsNullOrEmpty(product.Name) || string.IsNullOrEmpty(product.Description))
                return new ApiResponse<ProductDto>(false, null!, Messages.EmptyRequest);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var codeProduct = await GenerateProductCode();
                var newProduct = _mapper.Map<Product>(product);
                newProduct.Code = codeProduct;
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


        public async Task<ApiResponse<ProductDto>> UpdateProduct(int id, UpdateProductDto product)
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
                existingProduct.Code = product.Code;
                existingProduct.IsActive = product.IsActive;

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

                var productPrice = await _unitOfWork.Repository<ProductPrice>().GetFirstOrDefaultAsync(p => p.ProductId == id);
                if (productPrice == null)
                {
                    _unitOfWork.Repository<Product>().Delete(findProduct);
                    await _unitOfWork.SaveChangesAsync();
                    return new ApiResponse<bool>(true, true, Messages.ProductDeleted);
                }

                findProduct.IsActive = false;
                _unitOfWork.Repository<Product>().Update(findProduct);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<bool>(true, true, Messages.ProductStatusModifier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto: {Message}", ex.Message);
                return new ApiResponse<bool>(false, false, Messages.ErrorOccurred);
            }
        }



        private async Task<Dictionary<int, int>> GetQuantities()
        {
            var productPrices = await _unitOfWork.Repository<ProductPrice>().GetAllAsync();
            var quantities = productPrices
                .GroupBy(pp => pp.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(pp => pp.Quantity));

            return quantities;
        }

        private async Task<int> GetQuantityById(int id)
        {
            var productPrice = await _unitOfWork.Repository<ProductPrice>().FindAsync(p => p.ProductId == id);
            var quantity = productPrice.Sum(pp => pp.Quantity);

            return quantity;
        }

        private async Task<decimal> GetActualPrice(int id)
        {
            var product = await _unitOfWork.Repository<ProductPrice>().FindAsync(p => p.ProductId == id);
            var actualPrice = product
                .OrderByDescending(p => p.EntryDate)
                .FirstOrDefault();
            return actualPrice?.Price ?? 0;
        }

        private async Task<string> GenerateProductCode()
        {
            var lastProduct = await _unitOfWork.Repository<Product>()
                .GetQueryable()
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
            if (lastProduct == null)
            {
                return $"{PRODUCT_PREFIX}0001";
            }
            var lastId = int.Parse(lastProduct.Code.Substring(PRODUCT_PREFIX.Length));
            var newId = lastId + 1;
            return $"{PRODUCT_PREFIX}{newId:D4}";
        }

    }
}
