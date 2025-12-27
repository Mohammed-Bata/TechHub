using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, int>
    {
        private readonly IAppDbContext _context;
        public DeleteAddressCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
           

            var address = _context.Addresses.FirstOrDefault(a => a.Id == request.Id);

           

            if (address == null)
            {
                throw new NotFoundException("Address not found");
            }
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync(cancellationToken);
            return address.Id;
        }
    }
}
