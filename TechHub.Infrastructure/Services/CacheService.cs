using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;

namespace TechHub.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            string? data = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data))
            {
                return default(T);
            }
        ;
            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task InvalidatePaginatedCache(string baseKey, int pageSize, int total)
        {
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
            {
                string cacheKey = $"{baseKey}_{pageSize}_{pageNumber}";
                await _cache.RemoveAsync(cacheKey);
            }
        }

        public async Task InvalidatePaginatedCache(string baseKey, string one, int pageSize, int total)
        {
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
            {
                string cacheKey = $"{baseKey}_{one}_{pageSize}_{pageNumber}";
                await _cache.RemoveAsync(cacheKey);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                Console.WriteLine($"Error removing cache: {ex.Message}");
            }
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var data = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, data);
        }
    }
}
