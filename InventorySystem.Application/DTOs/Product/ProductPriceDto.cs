namespace InventorySystem.Application.DTOs.Product
{
    public class ProductPriceDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public decimal Price { get; set; }
    }
}
