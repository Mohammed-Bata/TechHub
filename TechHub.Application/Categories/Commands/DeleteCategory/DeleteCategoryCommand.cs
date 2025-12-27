using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(int Id): IRequest<int>;

}
