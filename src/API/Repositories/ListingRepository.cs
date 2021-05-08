using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ListingRepository> _logger;

        public ListingRepository(ApplicationDbContext dbContext, ILogger<ListingRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Listing>> GetListingsAsync(GetListingsRequest request)
        {
            var query = await Task.Run(() =>_dbContext.Listings.Where(x => 
                                           (string.IsNullOrEmpty(request.Suburb) || x.Suburb == request.Suburb)
                                        && (request.CategoryType == null || x.CategoryType == request.CategoryType)
                                        && (request.StatusType == null || x.StatusType == request.StatusType)));

            if (request.Skip > 0)
                query.Skip(request.Skip);

            if (request.Take > 0)
                query.Take(request.Take);

            _logger.LogInformation($"{query.Count()} retrieved");

            return query;
        }
    }
}
