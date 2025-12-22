using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;
using TechHub.Domain;
using Address = TechHub.Domain.Address;
using Review = TechHub.Domain.Review;

namespace TechHub.Infrastructure.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly AppDbContext _context;
       
        public IUserRepository Users { get; private set; }
        public IWishlistRepository Wishlists { get; private set; }
        public IRepository<Address> Addresses { get; private set; }
        public IRepository<Category> Categories { get; private set; }
        public IRepository<Order> Orders { get; private set; }
        public ICartRepository Carts { get; private set; }
        public IProductRepository Products { get; private set; }
        public IRepository<OrderItem> OrderItems { get; private set; }
        public IRepository<ShoppingCartItem> CartItems { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IRepository<ProductImage> ProductImages { get; private set; }

        public UnitOfWork(AppDbContext context, IWishlistRepository wishlists,IRepository<Address> addresses,
            IRepository<Category> categories, IRepository<Order> orders, ICartRepository carts,
            IProductRepository products, IRepository<OrderItem> orderitems, IReviewRepository reviews,
            IRepository<ProductImage> productImages, IUserRepository users)
        {
            _context = context;
            Wishlists = wishlists;
            Addresses = addresses;
            Categories = categories;
            Orders = orders;
            Carts = carts;
            Products = products;
            OrderItems = orderitems;
            Reviews = reviews;
            ProductImages = productImages;
            Users = users;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

      
    }
}
