using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Addresses.Queries.GetAddress
{
    public record GetAddressQuery(int Id,string UserId): IRequest<AddressDto>;

}
