using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
    {
        private readonly IAppDbContext _context;
        public DeleteReviewCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == request.ReviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
               throw new NotFoundException("Review Not Found");
            }
                return;
        }
    }
}
