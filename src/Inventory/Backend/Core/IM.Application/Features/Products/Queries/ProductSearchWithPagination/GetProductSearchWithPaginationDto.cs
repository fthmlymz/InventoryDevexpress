namespace InventoryManagement.Application.Features.Products.Queries.ProductSearchWithPagination
{
    public sealed class GetProductSearchWithPaginationDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Barcode { get; set; }
        public string? SerialNumber { get; set; }
        public string? Imei { get; set; }
        public string? Mac { get; set; }
        public string? DataClass { get; set; }
        public string? Status { get; set; }
        public string? AssignedUserName { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? FullName { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public DateTime? ProductDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? CategorySubId { get; set; }
        public string? CategorySubName { get; set; }
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }
        public int? ModelId { get; set; }
        public string? ModelName { get; set; }



        #region Relationship - Affiliated with the upper class
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        #endregion
    }
}
