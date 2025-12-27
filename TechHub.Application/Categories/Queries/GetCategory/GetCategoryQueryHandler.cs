using MediatR;
using Dapper;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Categories.Queries.GetCategory
{
    public class GetCategoryQueryHandler: IRequestHandler<GetCategoryQuery, CategoryDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public GetCategoryQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT Name
                           FROM Categories
                           WHERE Id = @Id";

            using var connection = _sqlConnectionFactory.CreateConnection();

            var result = await connection.QuerySingleOrDefaultAsync<CategoryDto>(sql, new { request.Id });

            if (result == null) {
                throw new NotFoundException($"Category with Id {request.Id} was not found.");
            }

            return result;

        }

    }
}
