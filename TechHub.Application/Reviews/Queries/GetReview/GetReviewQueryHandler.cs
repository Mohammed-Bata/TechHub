using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Reviews.Queries.GetReview
{
    public class GetReviewQueryHandler: IRequestHandler<GetReviewQuery, ReviewDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public GetReviewQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<ReviewDto> Handle(GetReviewQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT r.Content, r.Rating
                                 FROM Reviews r
                                 WHERE r.Id = @ReviewId;";
            using var connection = _sqlConnectionFactory.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<ReviewDto>(sql, new { request.ReviewId});
            return result;
        }
    }
}
