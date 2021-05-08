using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace API.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ListingRepository> _logger;

        private const int ExpirationTimeInSeconds = 5;

        public ListingRepository(ApplicationDbContext dbContext, IMemoryCache cache, ILogger<ListingRepository> logger)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<Listing>> GetListingsAsync(GetListingsRequest request)
        {
            string key = request.Suburb?.ToLower() ?? "all";

            var resultsBySuburb = await _cache.GetOrCreateAsync(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(ExpirationTimeInSeconds);
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(ExpirationTimeInSeconds);

                _logger.LogInformation($"Retrieving results from database key : {key}");

                return _dbContext.Listings.Where(x => request.Suburb == null || x.Suburb.ToLower() == request.Suburb.ToLower()).ToListAsync();
            });

            var results = resultsBySuburb.Where(x => (request.CategoryType == null || x.CategoryType == request.CategoryType)
                                        && (request.StatusType == null || x.StatusType == request.StatusType));

            if (request.Skip > 0)
                results = results.Skip(request.Skip);

            if (request.Take > 0)
                results = results.Take(request.Take);

            _logger.LogInformation($"{results.Count()} retrieved");

            return results;
        }
    }
}
