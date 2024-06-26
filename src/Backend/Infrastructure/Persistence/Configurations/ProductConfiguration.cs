﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.Barcode).IsRequired();
            builder.Property(x => x.SerialNumber).HasMaxLength(250);
            builder.Property(x => x.Imei).HasMaxLength(250);
            builder.Property(x => x.Mac).HasMaxLength(250);
            builder.Property(x => x.DataClass).HasMaxLength(250);
            builder.Property(x => x.Status).HasMaxLength(250);
            builder.Property(x => x.WorkflowId).HasMaxLength(250);
            builder.Property(x => x.CreatedUserId).HasMaxLength(250);
            builder.Property(x => x.UpdatedUserId).HasMaxLength(250);

            builder.Property(x => x.CreatedBy).HasMaxLength(250);
            builder.Property(x => x.UpdatedBy).HasMaxLength(250);

            builder.ToTable(nameof(Product));

            builder.HasOne(x => x.Company).WithMany(x => x.Products).HasForeignKey(x => x.CompanyId);

            // Product ile AssignedProduct arasındaki ilişki
            builder.HasMany(x => x.AssignedProducts)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product ile ProductMovement arasındaki ilişki
            builder.HasMany(x => x.ProductMovements)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            // Barcode için benzersiz indeks ekleniyor
            builder.HasIndex(x => x.Barcode).IsUnique();
        }
    }
}
