using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain;

namespace TechHub.Infrastructure.Configurations
{
    internal class ProductWishlistConfiguration : IEntityTypeConfiguration<ProductWishlist>
    {
        public void Configure(EntityTypeBuilder<ProductWishlist> builder)
        {
            builder.HasKey(pw => new { pw.ProductId, pw.WishlistId });
            builder.HasOne(pw=>pw.Product)
                .WithMany()
                .HasForeignKey(pw => pw.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Wishlist>() 
                .WithMany(w => w.Products)
                .HasForeignKey(pw => pw.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
    
}
