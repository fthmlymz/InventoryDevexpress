using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.CreatedUserId).HasMaxLength(250);
            builder.Property(x => x.UpdatedUserId).HasMaxLength(250);

            builder.ToTable(nameof(Category));

            //builder.HasOne(x => x.Company).WithMany(x => x.Categories).HasForeignKey(x => x.CompanyId);
        }
    }
}
