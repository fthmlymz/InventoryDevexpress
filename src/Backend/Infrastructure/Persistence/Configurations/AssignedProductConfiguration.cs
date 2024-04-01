using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Persistence.Configurations
{
    public class AssignedProductConfiguration : IEntityTypeConfiguration<AssignedProduct>
    {
        public void Configure(EntityTypeBuilder<AssignedProduct> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.AssignedUserId).HasMaxLength(250);
            builder.Property(x => x.AssignedUserName).HasMaxLength(250);
            builder.Property(x => x.FullName).HasMaxLength(250);
            builder.Property(x => x.ApprovalStatus).HasMaxLength(250);
            builder.Property(x => x.Company).HasMaxLength(250);
            builder.Property(x => x.PhysicalDeliveryOfficeName).HasMaxLength(250);
            builder.Property(x => x.Title).HasMaxLength(250);
            builder.Property(x => x.Manager).HasMaxLength(250);
            builder.Property(x => x.Department).HasMaxLength(250);
            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.CreatedUserId).HasMaxLength(250);
            builder.Property(x => x.UpdatedUserId).HasMaxLength(250);



            builder.ToTable(nameof(AssignedProduct));

            builder.HasOne(x => x.Product)
                .WithMany(x => x.AssignedProducts)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /*public void Configure(EntityTypeBuilder<AssignedProduct> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.AssignedUserId).HasMaxLength(250);
            builder.Property(x => x.AssignedUserName).HasMaxLength(250);
            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);
            builder.Property(x => x.CreatedUserId).HasMaxLength(250);
            builder.Property(x => x.UpdatedUserId).HasMaxLength(250);

            builder.ToTable(nameof(AssignedProduct));

            // AssignedProduct ile Product arasında ilişkiyi kurma
            builder.HasOne(x => x.Product)
                .WithMany(x => x.AssignedProducts)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }*/

        //public void Configure(EntityTypeBuilder<AssignedProduct> builder)
        //{
        //    builder.HasKey(x => x.Id);
        //    builder.Property(x => x.Id).UseIdentityColumn();
        //    builder.Property(x => x.AssignedUserId).HasMaxLength(250);
        //    builder.Property(x => x.AssignedUserName).HasMaxLength(250);
        //    builder.Property(x => x.CreatedBy).HasMaxLength(250);
        //    builder.Property(x => x.UpdatedBy).HasMaxLength(250);
        //    builder.Property(x => x.CreatedUserId).HasMaxLength(250);
        //    builder.Property(x => x.UpdatedUserId).HasMaxLength(250);

        //    builder.ToTable(nameof(AssignedProduct));

        //    //Bu alana relationship ayarlanacak.
        //    //Bu alanda Product ı birden fazla kullanıcı kullanabilecek şekilde ayarlandı
        //    builder.HasOne(x => x.Product) // Product tablosu ile ilişki belirtiliyor
        //            .WithMany() // Bir Product, birden fazla AssignedProduct'a sahip olabilir
        //            .HasForeignKey(x => x.ProductId); // ProductId alanı ile ilişkilendiriliyor
        //}
    }
}
