using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain;

namespace TechHub.Application.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task AddReview(Review review);
        Task DeleteReview(int reviewId);
        Task UpdateReview(Review review);
    }
}
