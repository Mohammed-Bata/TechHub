using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain;

namespace TechHub.Application.Interfaces
{
    public interface IProductRepository: IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProducts(
            Func<IQueryable<Product>, IQueryable<Product>>? includes = null,
            string? name = null,
            int? categoryId = null,
            string? description = null,
            string? brand = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? reviewCount = null,
            decimal? minAverageRating = null,
            int pageSize = 0, int pageNumber = 0);
    }
}
