using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Utilities;

namespace InventorySystem.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<ApiResponse<ProductWithPriceDto?>> GetProductWithDetails(int productId);
        Task<ApiResponse<IEnumerable<ProductQuantityDto>>> GetAll();
        Task<ApiResponse<ProductDto>> AddProduct(AddProductDto product);
        Task<ApiResponse<ProductDto>> UpdateProduct(int id, AddProductDto product);
        Task<ApiResponse<bool>> DeleteProduct(int id);
    }
}
