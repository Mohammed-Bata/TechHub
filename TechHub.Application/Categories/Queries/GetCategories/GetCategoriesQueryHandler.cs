using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        ISqlConnectionFactory _sqlConnectionFactory;
        public GetCategoriesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT Name
                           FROM Categories";
            using var connection = _sqlConnectionFactory.CreateConnection();
            var result = await connection.QueryAsync<CategoryDto>(sql);

            return result.ToList();
        }
    }
}
