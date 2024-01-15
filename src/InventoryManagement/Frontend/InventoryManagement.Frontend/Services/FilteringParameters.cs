namespace InventoryManagement.Frontend.Services
{
    public class FilteringParameters
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? Status { get; set; }
        public string? CompanyName { get; set; }
        public string? AssignedUserName { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? FullName { get; set; }
//        public int? CompanyId { get; set; }
    }
}
