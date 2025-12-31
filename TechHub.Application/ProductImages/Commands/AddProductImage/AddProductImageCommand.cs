using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.ProductImages.Commands.AddProductImage
{
    public record AddProductImageCommand(Guid ProductId,ProductImageDto ImageDto,string baseUrl): IRequest<int>;

}
