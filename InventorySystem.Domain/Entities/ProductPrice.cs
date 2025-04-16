namespace InventorySystem.Domain.Entities
{
    public class ProductPrice
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal Price { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
    }
}
