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
    internal class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.HasKey(sci => sci.Id);
            builder.Property(sci => sci.Id).ValueGeneratedOnAdd();
            builder.Property(sci => sci.Price).HasColumnType("decimal(18,2)");
            builder.Property(sci => sci.Quantity).IsRequired();
            builder.HasOne<ShoppingCart>()
                .WithMany(sc => sc.Items)
                .HasForeignKey(sci => sci.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(sci=>sci.Product)
                .WithMany()
                .HasForeignKey(sci => sci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
