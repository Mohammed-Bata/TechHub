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
    internal class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.Id).ValueGeneratedOnAdd();
            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<ShoppingCart>(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                
        }
    }
}
