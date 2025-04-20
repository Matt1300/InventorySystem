using InventorySystem.Application.DTOs.Auth;
using InventorySystem.Application.DTOs.Product;
using InventorySystem.Application.DTOs.User;
using InventorySystem.Application.Interfaces;
using InventorySystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _auth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuth auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AddProductDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] NewUserDto request)
        {
            _logger.LogInformation("Registrando nuevo usuario...");
            var response = await _auth.Register(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AddProductDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] UserDto request)
        {
            _logger.LogInformation("Iniciando Sesión...");
            var response = await _auth.Login(request);
            
            return Ok(response);
        }

        [HttpPost("validate-refresh-token")]
        public async Task<ActionResult<LoginResponseDto>> ValidateRefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            _logger.LogInformation("Validando refresh token...");
            var response = await _auth.ValidateAndRefreshToken(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Usuario Autorizado");
        }
    }
}
