using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;

namespace TechHub.Application.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler
    {
        private readonly IAppDbContext _context;
        public DeleteReviewCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == request.ReviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync(cancellationToken);
            }
            return request.ReviewId;
        }
    }
}
