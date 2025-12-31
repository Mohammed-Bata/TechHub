using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Domain.Entities;

namespace TechHub.Application.Wishlists.Queries.GetWishlist
{
    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, WishlistDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetWishlistQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<WishlistDto> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            var sql = @"
                SELECT w.Id,
                        w.Name AS WishlistName, 
                       w.CreatedAt AS WishlistCreatedAt,
                        p.Id AS ProductId,
                       p.Name,
                       p.Description, 
                       p.Price,
                       p.StockAmount,
                        p.Brand,
                        c.Id AS CategoryId,
                       c.Name AS CategoryName,
                       p.ImageLocalPath,
                       p.ImageUrl,
                       p.ProductCode,
                       p.AverageRating
                FROM Wishlists w
                LEFT JOIN ProductWishlists wp ON w.Id = wp.WishlistId
                LEFT JOIN Products p ON wp.ProductId = p.Id
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                WHERE w.UserId = @UserId;
            ";


            var wishlistDictionary = new Dictionary<Guid, WishlistDto>();
            var result = await connection.QueryAsync<WishlistDto, ProductWishlistDto, WishlistDto>(
                sql,
               (wishlist, product) =>
                {
                    if (!wishlistDictionary.TryGetValue(wishlist.Id, out var wishlistEntry))
                    {
                        wishlistEntry = new WishlistDto
                        {
                            Id = wishlist.Id,
                            Name = wishlist.Name,
                            Products = new List<ProductWishlistDto>()
                        };
                        wishlistDictionary.Add(wishlist.Id, wishlistEntry);
                    }
                    if (product != null)
                    {
                        wishlistEntry.Products.Add(product);
                    }
                    return wishlistEntry;
                },
                        new { UserId = request.UserId },
                        splitOn: "ProductId"
                    );
            return wishlistDictionary.Values.FirstOrDefault() ?? new WishlistDto { Name = "Default Wishlist", Products = new List<ProductWishlistDto>() };

        }
    }
}
