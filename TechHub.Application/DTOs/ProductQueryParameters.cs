using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public record ProductQueryParameters(
         string? Name,
         int? CategoryId,
         string? Description,
         string? Brand,
         decimal? MinPrice,
         decimal? MaxPrice,
         int? ReviewCount,
         decimal? MinAverageRating
        );
    
}
