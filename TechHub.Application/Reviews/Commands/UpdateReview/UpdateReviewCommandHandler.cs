using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;

namespace TechHub.Application.Reviews.Commands.UpdateReview
{
    public class UpdateReviewCommandHandler: IRequestHandler<UpdateReviewCommand,Guid>
    {
        private readonly IAppDbContext _context;
        public UpdateReviewCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == request.Id);
            if (review != null)
            {
                review.Content = request.Content;
                review.Rating = request.Rating;
                await _context.SaveChangesAsync(cancellationToken);
            }
            return request.Id;
        }
    }
}
