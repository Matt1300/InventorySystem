﻿namespace InventorySystem.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
