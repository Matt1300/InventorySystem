using AutoMapper;
using InventorySystem.Application.Constants;
using InventorySystem.Application.DTOs.Batch;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Interfaces;
using InventorySystem.Application.Interfaces.UnitOfWork;
using InventorySystem.Application.Utilities;
using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventorySystem.Application.Implementations
{
    public class Batch : IBatch
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepository> _logger;
        private const string BATCH_PREFIX = "BCH-";

        public Batch(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<BatchDto>>> GetAllBatches()
        {
            try
            {
                var batches = await _unitOfWork.Repository<ProductPrice>()
                    .GetQueryable()
                    .Include(b => b.Product)
                    .ToListAsync();

                var batchesDto = _mapper.Map<IEnumerable<BatchDto>>(batches);

                return new ApiResponse<IEnumerable<BatchDto>>(true, batchesDto, Messages.BatchesRetrieved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los lotes: {Message}", ex.Message);
                return new ApiResponse<IEnumerable<BatchDto>>(false, null!, Messages.ErrorOccurred);
            }
        }

        public async Task<ApiResponse<BatchDto>> AddBatch(CreateBatchDto batch)
        {
            if (batch == null || batch.ProductId <= 0 || batch.Quantity < 0 || batch.Price < 0)
                return new ApiResponse<BatchDto>(false, null!, Messages.EmptyRequest);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existProduct = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(p => p.Id == batch.ProductId);
                if (existProduct == null)
                    return new ApiResponse<BatchDto>(false, null!, Messages.ProductNotFound);

                var batchNumber = await GenerateBatchNumber();
                var newBatch = _mapper.Map<ProductPrice>(batch);
                newBatch.BatchNumber = batchNumber;
                await _unitOfWork.Repository<ProductPrice>().AddAsync(newBatch);
                await _unitOfWork.SaveChangesAsync();

                var batchDto = _mapper.Map<BatchDto>(newBatch);
                batchDto.NameProduct = existProduct.Name;
                await _unitOfWork.CommitAsync();

                return new ApiResponse<BatchDto>(true, batchDto, Messages.BatchAdded);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error al agregar un nuevo lote: {Message}", ex.Message);
                return new ApiResponse<BatchDto>(false, null!, Messages.ErrorOccurred);
            }

        }

        public async Task<ApiResponse<BatchDto>> UpdateBatch(int id, UpdateBatchDto batch)
        {
            if (id < 0 || batch is null)
                return new ApiResponse<BatchDto>(false, null!, Messages.EmptyRequest);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existBatch = await _unitOfWork.Repository<ProductPrice>()
                    .GetFirstOrDefaultAsync(b => b.Id == id);
                if (existBatch == null)
                    return new ApiResponse<BatchDto>(false, null!, Messages.BatchNotFound);

                existBatch.Price = batch.Price;
                existBatch.Quantity = batch.Quantity;
                existBatch.EntryDate = batch.EntryDate;

                _unitOfWork.Repository<ProductPrice>().Update(existBatch);
                await _unitOfWork.SaveChangesAsync();
                var batchDto = _mapper.Map<BatchDto>(existBatch);
                await _unitOfWork.CommitAsync();

                return new ApiResponse<BatchDto>(true, batchDto, Messages.BatchUpdated);

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar un lote existente: {Message}", ex.Message);
                return new ApiResponse<BatchDto>(false, null!, Messages.ErrorOccurred);
            }

        }

        public async Task<ApiResponse<bool>> DeleteBatch(int id)
        {
            if (id < 0)
                return new ApiResponse<bool>(false, false, Messages.EmptyRequest);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existBatch = await _unitOfWork.Repository<ProductPrice>()
                    .GetFirstOrDefaultAsync(b => b.Id == id);

                if (existBatch == null)
                    return new ApiResponse<bool>(false, false, Messages.BatchNotFound);

                _unitOfWork.Repository<ProductPrice>().Delete(existBatch);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return new ApiResponse<bool>(true, true, Messages.BatchDeleted);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar un lote existente: {Message}", ex.Message);
                return new ApiResponse<bool>(false, false, Messages.ErrorOccurred);
            }
        }


        private async Task<string> GenerateBatchNumber()
        {
            var lastBatch = await _unitOfWork.Repository<ProductPrice>()
                .GetQueryable()
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
            if (lastBatch == null)
            {
                return $"{BATCH_PREFIX}0001";
            }
            var lastId = int.Parse(lastBatch.BatchNumber.Substring(BATCH_PREFIX.Length));
            var newId = lastId + 1;
            return $"{BATCH_PREFIX}{newId:D4}";
        }
    }
}

