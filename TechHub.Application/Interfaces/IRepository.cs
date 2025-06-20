using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TechHub.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);

        Task<T> GetAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>>? includes = null
        );

        Task RemoveAsync(Expression<Func<T, bool>> filter);

        Task<IEnumerable<T>> GetAll(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int pageSize = 0,
            int pageNumber = 0
        );

        Task UpdateAsync(T entity);
    }
}

