namespace InventoryManagement.Frontend.Services
{
    public class FilteringParameters
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }


        //public int PageNumber { get; set; }
        //public int PageSize { get; set; }
        public string? Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? Status { get; set; }
        public int? CompanyId { get; set; }
    }
}
