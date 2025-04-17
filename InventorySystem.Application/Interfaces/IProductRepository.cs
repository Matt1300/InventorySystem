using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Utilities;

namespace InventorySystem.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<ApiResponse<ProductWithPriceDto?>> GetProductDtoWithDetailsAsync(int productId);
        Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync();
        Task<ApiResponse<ProductDto?>> GetByIdAsync(int id);
        Task<ApiResponse<bool>> AddAsync(ProductDto product);
        Task<ApiResponse<bool>> UpdateAsync(ProductDto product);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
