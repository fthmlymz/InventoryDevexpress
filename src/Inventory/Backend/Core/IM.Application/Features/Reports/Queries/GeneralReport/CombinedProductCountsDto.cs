namespace IM.Application.Features.Reports.Queries.GeneralReport
{
    public class CombinedProductCountsDto
    {
        public List<CompanyProductCountsDto>? CompanyProductReport { get; set; }
        public List<ProductCountsAllDto>? AllProductReport { get; set; }

        /*public class ProductCountsDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public int ProductCount { get; set; }
        }*/
    }
    public class ProductCountsAllDto //: CombinedProductCountsDto.ProductCountsDto
    {
        public int CategorySubId { get; set; }
        public string? CategorySubName { get; set; }
        public int ProductCount { get; set; }
    }

    public class CompanyProductCountsDto //: CombinedProductCountsDto.ProductCountsDto
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int CategorySubId { get; set; }
        public string? CategorySubName { get; set; }
        public int ProductCount { get; set; }
    }
}
