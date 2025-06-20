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
    internal class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();
            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(a => a.Governorate)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20);
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
