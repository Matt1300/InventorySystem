namespace InventorySystem.Application.DTOs.Product
{
    public class NewProductDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
