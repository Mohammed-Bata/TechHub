using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler: IRequestHandler<CreateReviewCommand, Guid>
    {
        private readonly IAppDbContext _context;
        public CreateReviewCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = new Review
            {
                ProductId = request.ProductId,
                Content = request.Content,
                Rating = request.Rating,
                UserId = request.UserId,
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            return review.Id;
        }
    }

}
