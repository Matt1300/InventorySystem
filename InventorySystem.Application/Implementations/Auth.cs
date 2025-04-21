using AutoMapper;
using InventorySystem.Application.Constants;
using InventorySystem.Application.DTOs.Auth;
using InventorySystem.Application.DTOs.User;
using InventorySystem.Application.Interfaces;
using InventorySystem.Application.Interfaces.UnitOfWork;
using InventorySystem.Application.Utilities;
using InventorySystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InventorySystem.Application.Implementations
{
    public class Auth : IAuth
    {
        private readonly ILogger<Auth> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        public Auth(ILogger<Auth> logger, IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IOptions<JwtSettings> jwtOptions)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<ApiResponse<string>> Register(NewUserDto user)
        {
            try
            {
                var existUser = await _unitOfWork.Repository<User>().GetFirstOrDefaultAsync(u => u.Username == user.Username);
                if (existUser != null)
                    return new ApiResponse<string>(false, null!, Messages.UserAlreadyExists);

                var newUser = _mapper.Map<User>(user);

                var hashedPassword = new PasswordHasher<User>()
                .HashPassword(newUser, user.Password);

                newUser.PasswordHash = hashedPassword;

                await _unitOfWork.Repository<User>().AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<string>(true, null!, Messages.UserRegistered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando nuevo usuaraio: {Message}", ex.Message);
                return new ApiResponse<string>(false, null!, Messages.ErrorOccurred);
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> Login(UserDto user)
        {
            try
            {
                var searchUser = await _unitOfWork.Repository<User>()
                    .GetFirstOrDefaultAsync(u => u.Username == user.Username);

                if (searchUser == null)
                    return new ApiResponse<LoginResponseDto>(false, null!, Messages.LoginFailed);

                var passwordVerificationResult = new PasswordHasher<User>()
                    .VerifyHashedPassword(searchUser, searchUser.PasswordHash, user.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    LoginResponseDto response = await CreateTokenResponse(searchUser);

                    return new ApiResponse<LoginResponseDto>(true, response, Messages.SuccessLogin);
                }

                return new ApiResponse<LoginResponseDto>(false, null!, Messages.LoginFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al loguearse con el user {username}: {Message}", user.Username, ex.Message);
                return new ApiResponse<LoginResponseDto>(false, null!, Messages.ErrorOccurred);
            }
        }

        private async Task<LoginResponseDto> CreateTokenResponse(User? searchUser)
        {
            return new LoginResponseDto
            {
                Token = CreateToken(searchUser),
                RefreshToken = await GenerateAndSaveRefreshToken(searchUser)
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName)
           };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTime),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ApiResponse<LoginResponseDto>> ValidateAndRefreshToken(RefreshTokenRequestDto request)
        {
            try
            {
                var user = await ValidateRefreshToken(request.Id, request.RefreshToken);
                if (user == null)
                    return new ApiResponse<LoginResponseDto>(false, null!, Messages.InvalidToken);

                var response = await CreateTokenResponse(user);
                return new ApiResponse<LoginResponseDto>(true, response, Messages.ValidToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar el token: {Message}", ex.Message);
                return new ApiResponse<LoginResponseDto>(false, null!, Messages.ErrorOccurred);
            }
        }

        private async Task<User?> ValidateRefreshToken(int userId, string refreshToken)
        {
            var user = await _unitOfWork.Repository<User>().GetFirstOrDefaultAsync(u => u.Id == userId && u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null;
            }
            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();
            return refreshToken;
        }
    }
}
