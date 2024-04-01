using Domain.Common;

namespace Domain.Entities
{
    public class Company : BaseAuditableEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;


        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Brand> Brands { get; set; } = new List<Brand>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<TransferOfficier>? TransferOfficiers { get; set; } = new List<TransferOfficier>();
    }
}
