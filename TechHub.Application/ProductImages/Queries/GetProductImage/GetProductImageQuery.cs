using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.ProductImages.Queries.GetProductImage
{
    public record GetProductImageQuery(int ImageId): IRequest<ProductImageResponseDto>;

}
