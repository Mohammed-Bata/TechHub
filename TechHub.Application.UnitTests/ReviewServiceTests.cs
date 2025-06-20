using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Services;
using TechHub.Domain;

namespace TechHub.Application.UnitTests
{
    public class ReviewServiceTests
    {
        private readonly IUnitOfWork _unitOfWorkMock = NSubstitute.Substitute.For<IUnitOfWork>();
        private readonly ICacheService _cacheMock = NSubstitute.Substitute.For<ICacheService>();
        private readonly IReviewRepository _reviewRepositoryMock = NSubstitute.Substitute.For<IReviewRepository>();
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _unitOfWorkMock.Reviews.Returns(_reviewRepositoryMock);
            _reviewService = new ReviewService(_unitOfWorkMock, _cacheMock);   
        }

        [Fact]
        public async Task GetReviews_ShouldReturnCachedReviews_WhenCacheExists()
        {
            // Arrange
            int productId = 1;
            int pageSize = 5;
            int pageNumber = 1;
            var cachedReviews = new List<Review>
            {
                new Review { Id = 1, ProductId = productId, Rating = 5, Content = "Great product!" }
            };
            _cacheMock.GetAsync<List<Review>>($"reviews_{productId}_{pageSize}_{pageNumber}").Returns(cachedReviews);
            // Act
            var result = await _reviewService.GetReviews(productId, pageSize, pageNumber);
            // Assert
            Assert.Equal(cachedReviews, result);
        }

        [Fact]
        public async Task GetReviews_ShouldReturnReviewsFromRepository_WhenCacheDoesNotExist()
        {
            // Arrange
            int productId = 1;
            int pageSize = 5;
            int pageNumber = 1;
            var reviews = new List<Review>
            {
                new Review { Id = 1, ProductId = productId, Rating = 5, Content = "Great product!" }
            };
            _cacheMock.GetAsync<List<Review>>($"reviews_{productId}_{pageSize}_{pageNumber}").Returns((List<Review>)null);
            _reviewRepositoryMock.GetAll(Arg.Any<Expression<Func<Review, bool>>>(), pageSize: pageSize, pageNumber: pageNumber)
                .Returns(reviews);
            // Act
            var result = await _reviewService.GetReviews(productId, pageSize, pageNumber);
            // Assert
            Assert.Equal(reviews, result);
        }

        [Fact]
        public async Task PostReview_ShouldReturnNewReview_WhenUserHasNotReviewedProduct()
        {
            // Arrange
            int productId = 1;
            string userId = "testUser";
            var reviewDto = new ReviewDto("Great product!", 5);
            _reviewRepositoryMock.GetAsync(x => x.ProductId == productId && x.UserId == userId)
                .Returns((Review)null);
            // Act
            var result = await _reviewService.PostReview(productId, reviewDto, userId);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviewDto.Rating, result.Rating);
            Assert.Equal(reviewDto.Content, result.Content);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(productId, result.ProductId);
        }
    }
}
