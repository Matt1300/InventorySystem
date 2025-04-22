namespace InventorySystem.Application.DTOs.Batch
{
    public class UpdateBatchDto
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
