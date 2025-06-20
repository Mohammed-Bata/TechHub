using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain;

namespace TechHub.Application.Interfaces
{
    public interface ICartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> CreateCartAsync(string userId);
        Task<ShoppingCart> GetCartWithItemsAsync(string userId);
        Task<bool> RemoveItemAsync(string userId, int itemId);
        Task<ShoppingCartItem> AddItemAsync(string userId, int productId, int quantity);
    }
}
