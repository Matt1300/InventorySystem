namespace InventorySystem.Application.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        public required int Id { get; set; }
        public required string RefreshToken { get; set; }
    }
}
