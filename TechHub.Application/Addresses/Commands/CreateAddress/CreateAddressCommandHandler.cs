using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Addresses.Commands.CreateAddress
{
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, int>
    {
        private readonly IAppDbContext _context;
        public CreateAddressCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = new Address
            {
                Street = request.Street,
                City = request.City,
                Governorate = request.Governorate,
                PostalCode = request.PostalCode,
                UserId = request.UserId
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync(cancellationToken);
            return address.Id;
        }
    }
}
