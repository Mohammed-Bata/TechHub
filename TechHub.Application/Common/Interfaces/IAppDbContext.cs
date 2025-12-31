using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;

namespace TechHub.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<Address> Addresses {get; }
        public DbSet<Product> Products {get;}
        public DbSet<ShoppingCart> Carts {get;}
        public DbSet<Order> Orders {get;}
        public DbSet<Category> Categories {get;}
        public DbSet<Review> Reviews {get; }
        public DbSet<Wishlist> Wishlists {get; }
        public DbSet<AppUser> Users {get; }
        public DbSet<ProductImage> ProductImages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
