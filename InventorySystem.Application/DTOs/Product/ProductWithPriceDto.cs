namespace InventorySystem.Application.DTOs.Product
{
    public class ProductWithPriceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ProductPriceDto> Prices { get; set; } = new List<ProductPriceDto>();
    }
}
