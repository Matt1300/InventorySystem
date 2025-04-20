using InventorySystem.Application.DTOs.Auth;
using InventorySystem.Application.DTOs.User;
using InventorySystem.Application.Utilities;

namespace InventorySystem.Application.Interfaces
{
    public interface IAuth
    {
        Task<ApiResponse<string>> Register(NewUserDto user);
        Task<ApiResponse<LoginResponseDto>> Login(UserDto user);
        Task<ApiResponse<LoginResponseDto>> ValidateAndRefreshToken(RefreshTokenRequestDto request);
    }
}
