namespace InventorySystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; } 
        public required string FullName { get; set; }
        public required string PasswordHash { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
