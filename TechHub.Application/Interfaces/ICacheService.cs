using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value);

        Task RemoveAsync(string key);

        Task InvalidatePaginatedCache(string baseKey, int pageSize, int total);

        Task InvalidatePaginatedCache(string baseKey, string one, int pageSize, int total);
    }
}
