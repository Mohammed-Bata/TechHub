using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Addresses.Commands.DeleteAddress
{
    public record DeleteAddressCommand(int Id) : IRequest<int>;

}
