using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.ProductImages.Queries.GetProductImages
{
    public class GetProductImagesQueryHandler: IRequestHandler<GetProductImagesQuery, List<ProductImageResponseDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetProductImagesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<ProductImageResponseDto>> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT ProductId, ImageUrl, ImageLocalPath
                          FROM ProductImages
                          WHERE ProductId = @ProductId";
            using var connection = _sqlConnectionFactory.CreateConnection();
            var productImages = await connection.QueryAsync<ProductImageResponseDto>(query, new { ProductId = request.ProductId });
            return productImages.ToList();
        }
    }
}
