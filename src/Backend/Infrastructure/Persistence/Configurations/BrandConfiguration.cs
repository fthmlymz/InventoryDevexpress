using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.CreatedUserId).HasMaxLength(250);
            builder.Property(x => x.UpdatedUserId).HasMaxLength(250);


            builder.ToTable(nameof(Brand));
            //builder.HasOne(x => x.Company).WithMany(x => x.Brands).HasForeignKey(x => x.CompanyId);
        }
    }
}
