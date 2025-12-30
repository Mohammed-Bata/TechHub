using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Products.Queries.SearchProducts
{
    public record SearchProductsQuery(ProductQueryParameters paras) : IRequest<List<ProductResponseDto>>;
    
}
