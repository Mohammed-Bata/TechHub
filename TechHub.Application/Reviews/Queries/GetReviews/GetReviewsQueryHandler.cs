using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Reviews.Queries.GetReviews
{
    public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, List<ReviewDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetReviewsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<ReviewDto>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT r.Content, r.Rating, u.UserName AS ReviewerName
                                 FROM Reviews r
                                 JOIN AspNetUsers u ON r.UserId = u.Id
                                 WHERE r.ProductId = @ProductId
                                 ORDER BY r.CreatedAt DESC";
            using var connection = _sqlConnectionFactory.CreateConnection();
            var result = await connection.QueryAsync<ReviewDto>(sql, new { request.ProductId });
            return result.ToList();
        }
    }
}
