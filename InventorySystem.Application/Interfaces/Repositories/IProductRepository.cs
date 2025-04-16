using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetProductWithDetailsAsync(Guid productId);
    }
}
