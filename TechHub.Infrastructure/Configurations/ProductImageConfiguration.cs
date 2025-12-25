using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;

namespace TechHub.Infrastructure.Configurations
{
    internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi=> new { pi.ImageId, pi.ProductId });
            builder.Property(pi => pi.ImageId).ValueGeneratedOnAdd();

        }
    }
}
