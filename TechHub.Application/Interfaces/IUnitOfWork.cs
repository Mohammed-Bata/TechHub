using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;
using Address = TechHub.Domain.Entities.Address;
using Review = TechHub.Domain.Entities.Review;

namespace TechHub.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IWishlistRepository Wishlists { get; }
        IRepository<Address> Addresses { get; }
        IRepository<Category> Categories { get; }
        IRepository<Order> Orders { get; }
        ICartRepository Carts { get; }
        IProductRepository Products { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<ShoppingCartItem> CartItems { get; }
        IReviewRepository Reviews { get; }
        IRepository<ProductImage> ProductImages { get; }


        Task SaveChangesAsync();
    }
}
