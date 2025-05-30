﻿namespace InventorySystem.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Code { get; set; }
        public bool IsActive { get; set; }
    }
}
