namespace InventorySystem.Application.DTOs.Batch
{
    public class CreateBatchDto
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
