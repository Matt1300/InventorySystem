using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Utilities;

namespace InventorySystem.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<ApiResponse<ProductWithPriceDto?>> GetProductWithDetails(int productId);
        Task<ApiResponse<IEnumerable<ProductDto>>> GetAll();
        Task<ApiResponse<ProductDto?>> GetById(int id);
        Task<ApiResponse<bool>> AddProduct(ProductDto product);
        Task<ApiResponse<bool>> UpdateProduct(ProductDto product);
        Task<ApiResponse<bool>> DeleteProduct(int id);
    }
}
