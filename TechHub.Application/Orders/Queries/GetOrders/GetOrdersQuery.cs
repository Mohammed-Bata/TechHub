using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Orders.Queries.GetOrders
{
    public record GetOrdersQuery(string UserId) : IRequest<List<OrderResponseDto>>;

}
