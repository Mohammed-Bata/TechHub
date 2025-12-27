using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Addresses.Commands.CreateAddress
{
    public record CreateAddressCommand(string Street, string City, string Governorate, string PostalCode, string UserId) : IRequest<int>;

}
