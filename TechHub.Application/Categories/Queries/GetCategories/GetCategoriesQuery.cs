using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery: IRequest<List<CategoryDto>>;

}
