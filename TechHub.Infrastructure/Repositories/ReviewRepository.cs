using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;
using TechHub.Domain;

namespace TechHub.Infrastructure.Repositories
{
    public class ReviewRepository: Repository<Review>, IReviewRepository
    {
        private readonly AppDbContext _context;
        public ReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddReview(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await UpdateProductRating(review.ProductId);
        
        }
        public async Task DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await UpdateProductRating(review.ProductId);
                
                await Task.CompletedTask;
            }
            else
            {
                return;
            }
        }
        public async Task UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            await UpdateProductRating(review.ProductId);
            
            await Task.CompletedTask;
        }

        private async Task UpdateProductRating(int productId)
        {
            var product = await _context.Products.Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == productId);
            if (product != null)
            {
                if (product.Reviews != null && product.Reviews.Count > 0)
                {
                    product.ReviewCount = product.Reviews.Count;
                    product.AverageRating = (decimal)product.Reviews.Average(r => r.Rating);
                }
                else
                {
                    product.ReviewCount = 0;
                    product.AverageRating = 0;
                }
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                await Task.CompletedTask;
            }
            else
            {
                return;
            }
        }

    }
}
