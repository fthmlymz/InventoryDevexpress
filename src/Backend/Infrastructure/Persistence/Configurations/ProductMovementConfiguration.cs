using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ProductMovementConfiguration : IEntityTypeConfiguration<ProductMovement>
    {
        public void Configure(EntityTypeBuilder<ProductMovement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.MovementDate);
            builder.Property(x => x.Description);

            builder.ToTable(nameof(ProductMovement));

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductMovements)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }


        /*public void Configure(EntityTypeBuilder<ProductMovement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Description).HasMaxLength(250);
            builder.Property(x => x.MovementDate);

            builder.ToTable(nameof(ProductMovement));


            // ProductMovement ile Product arasında ilişkiyi kurma
            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductMovements)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }*/
    }

}
