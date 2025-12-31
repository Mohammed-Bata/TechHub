using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Orders.Queries.GetOrder
{
    public class GetOrderQueryHandler: IRequestHandler<GetOrderQuery, OrderResponseDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetOrderQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<OrderResponseDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            var sql = @"
                SELECT o.Id, o.AddressId,a.Street,a.City, o.OrderDate, o.TotalAmount, o.PaymentIntentId,
                       oi.Id AS ItemId, oi.ProductId, oi.Quantity, oi.UnitPrice,(oi.Quantity * oi.UnitPrice) AS TotalPrice, p.Name AS ProductName
                FROM Orders o
                LEFT JOIN Addresses a ON o.AddressId = a.Id
                LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
                LEFT JOIN Products p ON oi.ProductId = p.Id
                WHERE o.Id = @OrderId AND o.UserId = @UserId";

            var orderDictionary = new Dictionary<Guid, OrderResponseDto>();
            var result = await connection.QueryAsync<OrderResponseDto, OrderItemDto, OrderResponseDto>(
                sql,
                (order, orderItem) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }
                    if (orderItem != null)
                    {
                        orderEntry.OrderItems.Add(orderItem);
                    }
                    return orderEntry;
                },
                new { OrderId = request.Id, UserId = request.UserId },
                splitOn: "ItemId"
            );
            return orderDictionary.Values.FirstOrDefault();
        }


    }
}
