using MediatR;
using TechHub.Application.DTOs;

namespace TechHub.Application.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(Guid ProductId,ProductDto ProductDto,string baseUrl):IRequest<Guid>;

}
