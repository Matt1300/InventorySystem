﻿namespace InventorySystem.Application.DTOs.Product
{
    public class ProductQuantityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ActualPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
