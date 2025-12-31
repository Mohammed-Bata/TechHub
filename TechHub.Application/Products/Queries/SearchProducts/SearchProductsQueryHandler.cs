using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Products.Queries.SearchProducts
{
    public class SearchProductsQueryHandler: IRequestHandler<SearchProductsQuery, List<ProductResponseDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SearchProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<ProductResponseDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var connection = _sqlConnectionFactory.CreateConnection();
            var query = @"SELECT p.Id, p.Name, p.Description, p.Brand, p.Price, p.StockAmount , p.ProductCode, p.AverageRating, c.Id AS CategoryId, c.Name AS CategoryName, p.ImageUrl,
                           (SELECT STRING_AGG(ImageUrl, ',') 
                     FROM ProductImages 
                     WHERE ProductId = p.Id) AS ImageUrlsString
                          FROM Products p
                          INNER JOIN Categories c ON p.CategoryId = c.Id
                            LEFT JOIN ProductImages pi ON p.Id = pi.ProductId
                          WHERE (@Name IS NULL OR LOWER(p.Name) LIKE '%' + LOWER(@Name) + '%')
                            AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
                            AND (@Description IS NULL OR LOWER(p.Description) LIKE '%' + LOWER(@Description) + '%')
                            AND (@Brand IS NULL OR LOWER(p.Brand) LIKE '%' + LOWER(@Brand) + '%')
                            AND (@MinPrice IS NULL OR p.Price >= @MinPrice)
                            AND (@MaxPrice IS NULL OR p.Price <= @MaxPrice)
                            AND (@AverageRating IS NULL OR p.AverageRating >= @AverageRating);";
            
            var products = await connection.QueryAsync<ProductResponseDto>(query, new
                {
                Name = request.paras.Name,
                CategoryId = request.paras.CategoryId,
                Description = request.paras.Description,
                Brand = request.paras.Brand,
                MinPrice = request.paras.MinPrice,
                MaxPrice = request.paras.MaxPrice,
                AverageRating = request.paras.MinAverageRating
            });



            return products.ToList();

        }

    }
}
