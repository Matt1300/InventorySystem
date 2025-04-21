using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UpdateProductDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los productos...");
            var response = await _productRepository.GetAll();

            return Ok(response);

        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductWithPriceDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductsWithPrice(int id)
        {
            _logger.LogInformation("Obteniendo el producto con id {id}", id);
            var response = await _productRepository.GetProductWithDetails(id);

            return Ok(response);

        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProductDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> AddNewProduct(NewProductDto product)
        {
            _logger.LogInformation("Ingresando nuevo producto...");
            var response = await _productRepository.AddProduct(product);

            return Ok(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProductDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto product)
        {
            _logger.LogInformation("Actualizando producto...");
            var response = await _productRepository.UpdateProduct(id, product);

            return Ok(response);

        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Eliminando producto...");
            var response = await _productRepository.DeleteProduct(id);

            return Ok(response);

        }
    }
}
