namespace InventoryManagement.Frontend.DTOs.Products
{
    public class UpdateGetStoreProductCommandDto
    {
        public int Id { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
