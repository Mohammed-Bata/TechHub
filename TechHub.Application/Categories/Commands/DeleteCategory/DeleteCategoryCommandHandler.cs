using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;

namespace TechHub.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler: IRequestHandler<DeleteCategoryCommand, int>
    {
        private readonly IAppDbContext _context;
        public DeleteCategoryCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(DeleteCategoryCommand request,CancellationToken cancellationToken)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == request.Id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Id;
        }
    }
}
