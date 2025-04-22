using InventorySystem.Application.DTOs.Batch;
using InventorySystem.Application.Utilities;

namespace InventorySystem.Application.Interfaces
{
    public interface IBatch
    {
        Task<ApiResponse<IEnumerable<BatchDto>>> GetAllBatches();
        Task<ApiResponse<BatchDto>> AddBatch(CreateBatchDto batch);
        Task<ApiResponse<BatchDto>> UpdateBatch(int id, UpdateBatchDto batch);
        Task<ApiResponse<bool>> DeleteBatch(int id);
    }
}
