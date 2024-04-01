namespace InventoryManagement.Frontend.DTOs.Product
{
    public class AssignedProductDto
    {
        public int Id { get; set; }
        public string? AssignedUserName { get; set; }
        public string? AssignedUserId { get; set; }
        public string? AssignedUserPhoto { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? Barcode { get; set; }
        public string? ProductName { get; set; }
        public string? Company { get; set; }
        public string? PhysicalDeliveryOfficeName { get; set; }
        public string? Title { get; set; }
        public string? Manager { get; set; }
        public string? Department { get; set; }
        public int ProductId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
