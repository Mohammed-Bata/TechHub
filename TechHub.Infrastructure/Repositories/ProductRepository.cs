using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts(
           Func<IQueryable<Product>, IQueryable<Product>>? includes = null,
           string? name = null,
           int? categoryId = null,
           string? description = null,
           string? brand = null,
           decimal? minPrice = null,
           decimal? maxPrice = null,
           int? reviewCount = null,
           decimal? minAverageRating = null,
           int pageSize = 0, int pageNumber = 0)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category);

            // Apply filters based on the provided query parameters
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }
            if (categoryId != null)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(p => p.Description.ToLower().Contains(description.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.Brand.ToLower().Contains(brand.ToLower()));
            }
            if (minPrice != null)
            {
                query = query.Where(p => p.Price >= minPrice);
            }
            if (maxPrice != null)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }
            if (reviewCount != null)
            {
                query = query.Where(p => p.ReviewCount >= reviewCount);
            }
            if (minAverageRating != null)
            {
                query = query.Where(p => p.AverageRating >= minAverageRating);
            }

            // Apply sorting
            if (pageSize > 0 && pageNumber > 0)
            {
                return await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return await query.ToListAsync();
        }

    }

}
