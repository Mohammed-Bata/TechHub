using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Services
{
    public class ReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ReviewService(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<List<Review>> GetReviews(Guid productId, int pageSize = 5, int pageNumber = 1)
        {
            string cacheKey = $"reviews_{productId}_{pageSize}_{pageNumber}";
            var cachedReviews = await _cache.GetAsync<List<Review>>(cacheKey);
            if (cachedReviews != null)
            {
                return cachedReviews;
            }
            var reviews = await _unitOfWork.Reviews.GetAll(r => r.ProductId == productId, pageSize: pageSize, pageNumber: pageNumber);

            await _cache.SetAsync(cacheKey, reviews);

            return reviews.ToList();
        }

        public async Task<Review> PostReview(Guid productId,ReviewDto reviewDto,string userId)
        {
            var review = await _unitOfWork.Reviews.GetAsync(x => x.ProductId == productId && x.UserId == userId);
            if (review != null)
            {
                return null; // User already reviewed this product
            }
            var newreview = new Review()
            {
                Rating = reviewDto.Rating,
                Content = reviewDto.Content,
                UserId = userId,
                ProductId = productId
            };

            await _unitOfWork.Reviews.AddReview(newreview);
            await _unitOfWork.SaveChangesAsync();

            var rev = await _unitOfWork.Reviews.GetAll();
            var total = rev.Count();
            await _cache.InvalidatePaginatedCache("reviews", productId.ToString(), 5, total);
            
            return newreview;
        }

        public async Task<Review> GetReview(Guid id)
        {
            var review = await _unitOfWork.Reviews.GetAsync(x => x.Id == id);
            if (review == null)
            {
                return null;
            }
            return review;
        }

        public async Task<bool> PutReview(Guid id,ReviewDto reviewDto)
        {
                var review = await _unitOfWork.Reviews.GetAsync(x => x.Id == id);

                review.Content = reviewDto.Content;
                review.Rating = reviewDto.Rating;

                await _unitOfWork.Reviews.UpdateReview(review);
                await _unitOfWork.SaveChangesAsync();

                var productId = review.ProductId;

                var rev = await _unitOfWork.Reviews.GetAll();
                var total = rev.Count();
                await _cache.InvalidatePaginatedCache("reviews", productId.ToString(), 5, total);

                return true;
        }

        public async Task<bool> DeleteReview(Guid id)
        {

            var review = await _unitOfWork.Reviews.GetAsync(x => x.Id == id);

            var productId = review.ProductId;

            var rev = await _unitOfWork.Reviews.GetAll();

            //await _unitOfWork.Reviews.DeleteReview(id);
            await _unitOfWork.SaveChangesAsync();

            var total = rev.Count();
            await _cache.InvalidatePaginatedCache("reviews", productId.ToString(), 5, total);

            return true;
        }
    }
}
