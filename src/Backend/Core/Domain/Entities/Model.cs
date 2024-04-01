using Domain.Common;

namespace Domain.Entities
{
    public class Model : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;



        #region Relationship - Affiliated with the upper class
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        #endregion
    }
}
