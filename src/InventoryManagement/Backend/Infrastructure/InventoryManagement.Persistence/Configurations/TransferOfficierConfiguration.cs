using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Persistence.Configurations
{
    public class TransferOfficierConfiguration : IEntityTypeConfiguration<TransferOfficier>
    {
        public void Configure(EntityTypeBuilder<TransferOfficier> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.CreatedUserId).HasMaxLength(250);
            builder.Property(x => x.UpdatedUserId).HasMaxLength(250);
            builder.Property(x => x.UserName).HasMaxLength(250);
            builder.Property(x => x.FullName).HasMaxLength(250);
            builder.Property(x => x.Email).HasMaxLength(250);


            builder.ToTable(nameof(TransferOfficier));
            builder.HasOne(x => x.Company).WithMany(x => x.TransferOfficiers).HasForeignKey(x => x.CompanyId);
        }
    }
}
