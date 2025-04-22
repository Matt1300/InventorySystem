using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Utilities;

namespace InventorySystem.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<ApiResponse<IEnumerable<ProductQuantityDto>>> GetAll();
        Task<ApiResponse<ProductWithPriceDto?>> GetProductWithDetails(int productId);
        Task<ApiResponse<IEnumerable<SearchProductDto>>> SearchProduct(string term);
        Task<ApiResponse<ProductDto>> AddProduct(NewProductDto product);
        Task<ApiResponse<ProductDto>> UpdateProduct(int id, UpdateProductDto product);
        Task<ApiResponse<bool>> DeleteProduct(int id);
    }
}
