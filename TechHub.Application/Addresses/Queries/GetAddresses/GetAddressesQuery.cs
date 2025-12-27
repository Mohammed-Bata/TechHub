using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Addresses.Queries.GetAddresses
{
    public record GetAddressesQuery(string UserId): IRequest<List<AddressDto>>;

}
