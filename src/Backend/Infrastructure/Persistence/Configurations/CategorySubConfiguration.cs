using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class CategorySubConfiguration : IEntityTypeConfiguration<CategorySub>
    {
        public void Configure(EntityTypeBuilder<CategorySub> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.CreatedUserId).HasMaxLength(250);
            builder.Property(x => x.UpdatedUserId).HasMaxLength(250);

            builder.ToTable(nameof(CategorySub));

            builder.HasOne(x => x.Category).WithMany(x => x.CategorySubs).HasForeignKey(x => x.CategoryId);
        }
    }
}
