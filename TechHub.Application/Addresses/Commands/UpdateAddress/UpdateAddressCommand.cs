using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Addresses.Commands.UpdateAddress
{
    public record UpdateAddressCommand(int Id, string Street, string City, string Governorate, string PostalCode, string UserId) : IRequest<int>;

}
