using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Products.Queries.GetProduct
{
    public class GetProductQueryHandler: IRequestHandler<GetProductQuery, ProductResponseDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetProductQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<ProductResponseDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    p.Id, p.Name, p.Description, p.Price, p.StockAmount, p.Brand, p.CategoryId, c.Name AS CategoryName,
                    p.ImageUrl,
                    p.ProductCode,
                    p.AverageRating,
                   (SELECT STRING_AGG(ImageUrl, ',') 
                     FROM ProductImages 
                     WHERE ProductId = p.Id) AS ImageUrlsString
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                WHERE p.Id = @Id   
            ";
            using var connection = _sqlConnectionFactory.CreateConnection();
            var product = await connection.QuerySingleOrDefaultAsync<ProductResponseDto>(query, new { Id = request.Id });
            
            return product;
        }
    }
}
