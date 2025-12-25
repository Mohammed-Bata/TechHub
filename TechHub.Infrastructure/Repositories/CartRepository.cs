using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Infrastructure.Repositories
{
    public class CartRepository: Repository<ShoppingCart>, ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ShoppingCart> CreateCartAsync(string userId)
        {
            var cart = new ShoppingCart
            {
                UserId = userId,
            };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            return cart;
        }
        public async Task<ShoppingCart> GetCartWithItemsAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                cart = await CreateCartAsync(userId);
                cart = await _context.Carts
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.UserId == userId);
               
            }

            return cart;
        }
        public async Task<bool> RemoveItemAsync(string userId, Guid itemId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            var item = cart.Items.FirstOrDefault(x => x.Id == itemId);

            if (item is null)
            {
                return false;
            }
            cart.Items.Remove(item);
            _context.CartItems.Remove(item);

            
            return true;
        }
        public async Task<ShoppingCartItem> AddItemAsync(string userId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            var cart = await GetCartWithItemsAsync(userId);
            if (cart is null)
            {
                cart = await CreateCartAsync(userId);
            }

            var existCartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (existCartItem is null)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

                if (product is null)
                {
                    throw new ArgumentException("Product not found.");
                }

                var cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.TotalPrice
                };
                await _context.CartItems.AddAsync(cartItem);
                cart.Items.Add(cartItem);
            }
            else
            {
                existCartItem.Quantity += quantity;
                _context.CartItems.Update(existCartItem);
            }
            
            return existCartItem ?? cart.Items.FirstOrDefault(x => x.ProductId == productId);
        }
    }
}
