using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(ProductDto ProductDto,string baseUrl):IRequest<Guid>;
   
}
