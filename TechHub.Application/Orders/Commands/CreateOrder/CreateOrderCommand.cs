using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(string UserId,int AddressId):IRequest<Guid>;
  

}
