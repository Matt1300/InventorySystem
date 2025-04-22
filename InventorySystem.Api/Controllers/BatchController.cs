using InventorySystem.Application.DTOs.Batch;
using InventorySystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IBatch _batch;
        private readonly ILogger<BatchController> _logger;
        public BatchController(IBatch batch, ILogger<BatchController> logger)
        {
            _batch = batch;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BatchDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> GetAllBatches()
        {
            _logger.LogInformation("Obteniendo todos los lotes...");
            var response = await _batch.GetAllBatches();

            return Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatchDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost()]
        public async Task<IActionResult> AddBatch([FromBody] CreateBatchDto batch)
        {
            _logger.LogInformation("Agregando nueva compra de lote...");
            var response = await _batch.AddBatch(batch);

            return Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatchDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBatch(int id, [FromBody] UpdateBatchDto batch)
        {
            _logger.LogInformation("Actualizando lote con ID: {Id}", id);
            var response = await _batch.UpdateBatch(id, batch);

            return Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            _logger.LogInformation("Eliminando lote con ID: {Id}", id);
            var response = await _batch.DeleteBatch(id);

            return Ok(response);
        }
    }
}
