using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.ProductImages.Queries.GetProductImage
{
    public class GetProductImageQueryHandler: IRequestHandler<GetProductImageQuery, ProductImageResponseDto>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public GetProductImageQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<ProductImageResponseDto> Handle(GetProductImageQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT ProductId, ImageUrl, ImageLocalPath
                          FROM ProductImages
                          WHERE ImageId = @ImageId";
            using var connection = _sqlConnectionFactory.CreateConnection();
            var productImage = await connection.QuerySingleOrDefaultAsync<ProductImageResponseDto>(query, new { ImageId = request.ImageId });

            if(productImage == null)
            {
                throw new NotFoundException($"Product image with ID {request.ImageId} was not found.");
            }

            return productImage;
        }
    }
}
