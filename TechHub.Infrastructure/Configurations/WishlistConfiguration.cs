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
    internal class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id).ValueGeneratedOnAdd();
            builder.Property(w=>w.CreatedAt).HasColumnType("datetime");
            builder.HasOne<ApplicationUser>()
                   .WithOne()
                   .HasForeignKey<Wishlist>(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(w => w.Products)
                   .WithOne()
                   .HasForeignKey(pw => pw.WishlistId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
