namespace InventorySystem.Application.DTOs.Product
{
    public class AddProductDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Code { get; set; }
    }
}
