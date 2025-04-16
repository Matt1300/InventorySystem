using InventorySystem.Application.Interfaces.Repositories;

namespace InventorySystem.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
