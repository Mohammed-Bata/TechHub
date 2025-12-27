using MediatR;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler: IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly IAppDbContext _context;
        public CreateCategoryCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateCategoryCommand request,CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Id;
        }

    }
}
