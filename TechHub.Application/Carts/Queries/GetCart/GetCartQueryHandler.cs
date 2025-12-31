using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Carts.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, ShoppingCartDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetCartQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<ShoppingCartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            var sql = @"
                SELECT c.Id, 
                       ci.Id AS ItemId, ci.Quantity,
                       p.Id AS ProductId, p.Name AS ProductName, p.Price,
                        (ci.Price * ci.Quantity) as TotalPrice
                FROM Carts c
                LEFT JOIN CartItems ci ON c.Id = ci.ShoppingCartId
                LEFT JOIN Products p ON ci.ProductId = p.Id
                WHERE c.UserId = @UserId";
            var cartDictionary = new Dictionary<Guid, ShoppingCartDto>();
            var result = await connection.QueryAsync<ShoppingCartDto, ShoppingCartItemDto, ShoppingCartDto>(
                sql,
                (cart, item) =>
                {
                    if (!cartDictionary.TryGetValue(cart.Id, out var currentCart))
                    {
                        currentCart = cart;
                        cartDictionary.Add(currentCart.Id, currentCart);
                    }
                    if (item != null)
                    {
                        currentCart.Items.Add(item);
                    }
                    return currentCart;
                },
                new { UserId = request.UserId },
                splitOn: "ItemId"
            );
            return cartDictionary.Values.FirstOrDefault();
        }

    }
}
