using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;

namespace TechHub.Application.Interfaces
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
        Task<Wishlist> CreateWishlist(string userId);
        Task<Wishlist> GetWishlistByUserId(string userId);
        Task<bool> AddProductToWishlist(Guid productId, string userId);
        Task<bool> RemoveFromWishlist(Guid productId, string userId);
    }
}
