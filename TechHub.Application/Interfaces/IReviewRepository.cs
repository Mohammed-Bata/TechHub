using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;

namespace TechHub.Application.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task AddReview(Review review);
        Task DeleteReview(Guid reviewId);
        Task UpdateReview(Review review);
    }
}
