using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, int>
    {
        private readonly IAppDbContext _context;
        public UpdateAddressCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = _context.Addresses.FirstOrDefault(a => a.Id == request.Id && a.UserId == request.UserId);
            if (address == null)
            {
                throw new NotFoundException("Address not found");
            }
            address.Street = request.Street;
            address.City = request.City;
            address.Governorate = request.Governorate;
            address.PostalCode = request.PostalCode;

            await _context.SaveChangesAsync(cancellationToken);
            return address.Id;
        }
    }
}
