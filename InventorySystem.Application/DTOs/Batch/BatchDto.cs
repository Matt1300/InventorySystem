namespace InventorySystem.Application.DTOs.Batch
{
    public class BatchDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string NameProduct { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
    }
}
