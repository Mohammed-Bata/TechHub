using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;

namespace TechHub.Infrastructure.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
            builder.Property(p=>p.AverageRating).HasDefaultValue(0).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.StockAmount).IsRequired();
            builder.Property(p=>p.Brand).IsRequired().HasMaxLength(50);
            builder.HasOne(p=>p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(p => p.ProductCode).IsUnique();
            builder.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(p=>p.ProductCode).IsRequired().HasMaxLength(20);
            builder.HasIndex(p => p.ProductCode).IsUnique();

        }
    }
}
