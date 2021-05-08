using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API.Tests
{
    public class ListingRepositoryTests
    {
        private readonly ILogger<ListingRepository> _logger;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly ApplicationDbContext _dbContext;

        public ListingRepositoryTests()
        {
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _dbContext = new ApplicationDbContext(optionBuilder.Options);

            var mockLogger = new Mock<ILogger<ListingRepository>>();
            _logger = mockLogger.Object;

            _mockCache = new Mock<IMemoryCache>();
        }

        [Fact]
        public async Task WhenListingsExistInCacheThenReturnsFromCache()
        {
            var dummyListings = GetDummyListings();

            var cache = new MemoryCache(new MemoryCacheOptions());

            cache.Set("southbankdummy", dummyListings);

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest { Suburb = "southbankdummy" });

            Assert.Equal(dummyListings.Count(), listings.Count());
        }

        [Fact]
        public async Task WhenListingsDoesNotExistInCacheThenReturnsFromDatabase()
        {
            var dummyListings = GetDummyListings();

            _dbContext.Listings.AddRange(dummyListings);
            await _dbContext.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest { Suburb = "southbank" });

            Assert.Equal(dummyListings.Count(), listings.Count());
        }

        [Fact]
        public async Task WhenFilterByCategoryTypeThenReturnsFilteredResult()
        {
            var dummyListings = GetDummyListings();

            _dbContext.Listings.AddRange(dummyListings);
            await _dbContext.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest 
            { 
                Suburb = "southbank", 
                CategoryType = Enums.CategoryType.Residential
            });

            var result = listings.ToList();
            Assert.Equal(3, result.Count());
            Assert.Equal(Enums.CategoryType.Residential, result[0].CategoryType);
            Assert.Equal(Enums.CategoryType.Residential, result[1].CategoryType);
            Assert.Equal(Enums.CategoryType.Residential, result[2].CategoryType);
        }

        [Fact]
        public async Task WhenFilterByCategoryTypeAndStatusTypeThenReturnsFilteredResult()
        {
            var dummyListings = GetDummyListings();

            _dbContext.Listings.AddRange(dummyListings);
            await _dbContext.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest
            {
                Suburb = "southbank",
                CategoryType = Enums.CategoryType.Residential,
                StatusType = Enums.StatusType.Current
            });

            var result = listings.ToList();
            Assert.Equal(2, result.Count());
            Assert.Equal(Enums.CategoryType.Residential, result[0].CategoryType);
            Assert.Equal(Enums.CategoryType.Residential, result[1].CategoryType);
            Assert.Equal(Enums.StatusType.Current, result[0].StatusType);
            Assert.Equal(Enums.StatusType.Current, result[1].StatusType);
        }

        [Fact]
        public async Task WhenFilterByCategoryTypeAndStatusTypeAndTakeOneThenReturnsOnlyOneRecord()
        {
            var dummyListings = GetDummyListings();

            _dbContext.Listings.AddRange(dummyListings);
            await _dbContext.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest
            {
                Suburb = "southbank",
                CategoryType = Enums.CategoryType.Residential,
                StatusType = Enums.StatusType.Current,
                Take = 1
            });

            var result = listings.ToList();
            Assert.Single(result);
            Assert.Equal(Enums.CategoryType.Residential, result[0].CategoryType);
            Assert.Equal(Enums.StatusType.Current, result[0].StatusType);
            Assert.Equal(1096, result[0].ListingId);
        }

        [Fact]
        public async Task WhenFilterByCategoryTypeAndStatusTypeAndTakeOneAndSkipOneThenReturnsOnlyOneRecord()
        {
            var dummyListings = GetDummyListings();

            _dbContext.Listings.AddRange(dummyListings);
            await _dbContext.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest
            {
                Suburb = "southbank",
                CategoryType = Enums.CategoryType.Residential,
                StatusType = Enums.StatusType.Current,
                Take = 1,
                Skip = 1
            });

            var result = listings.ToList();
            Assert.Single(result);
            Assert.Equal(Enums.CategoryType.Residential, result[0].CategoryType);
            Assert.Equal(Enums.StatusType.Current, result[0].StatusType);
            Assert.Equal(1219, result[0].ListingId);
        }

        [Fact]
        public async Task WhenSkippedMoreItemsThanNoOfItemsRetrievedThenReturnsEmptyList()
        {
            var dummyListings = GetDummyListings();

            _dbContext.Listings.AddRange(dummyListings);
            await _dbContext.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest
            {
                Suburb = "southbank",
                CategoryType = Enums.CategoryType.Residential,
                StatusType = Enums.StatusType.Current,
                Take = 1,
                Skip = 2
            });

            var result = listings.ToList();
            Assert.Empty(result);
        }

        [Fact]
        public async Task WhenTakeMoreItemsThanNoOfItemsRetrievedThenReturnsWhateverIsAvailable()
        {
            var dummyListings = GetDummyListings();

            _dbContext.Listings.AddRange(dummyListings);
            await _dbContext.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var repo = new ListingRepository(_dbContext, cache, _logger);

            var listings = await repo.GetListingsAsync(new Models.GetListingsRequest
            {
                Suburb = "southbank",
                CategoryType = Enums.CategoryType.Residential,
                StatusType = Enums.StatusType.Current,
                Take = 3
            });

            var result = listings.ToList();
            Assert.Equal(2, result.Count());
        }

        private IEnumerable<Listing> GetDummyListings()
        {
            return new List<Listing> {
                    new Repositories.Entities.Listing
                    {
                        ListingId = 1096,
                        StreetNumber = "3204/241",
                        Street = "City Road",
                        Suburb = "Southbank",
                        State = "Victoria",
                        Postcode = 3006,
                        CategoryType = Enums.CategoryType.Residential,
                        StatusType = Enums.StatusType.Current,
                        DisplayPrice = "$600",
                        Title = "Sweeping views of the City all the way to Albert Park Lake! (Heating and Cooling in master bedroom and lounge)"
                    }, new Repositories.Entities.Listing
                    {
                        ListingId = 1131,
                        StreetNumber = "1208/83",
                        Street = "Queens Bridge Street",
                        Suburb = "Southbank",
                        State = "Victoria",
                        Postcode = 3006,
                        CategoryType = Enums.CategoryType.Rental,
                        StatusType = Enums.StatusType.Current,
                        DisplayPrice = "$575 per week",
                        Title = "575pw, min stay 6 months"
                    }, new Repositories.Entities.Listing
                    {
                        ListingId = 1147,
                        StreetNumber = "1/64",
                        Street = "Coventry Street",
                        Suburb = "Southbank",
                        State = "Victoria",
                        Postcode = 3006,
                        CategoryType = Enums.CategoryType.Residential,
                        StatusType = Enums.StatusType.Sold,
                        DisplayPrice = "$662,000",
                        Title = "Inner-City Garden Jewel"
                    }, new Repositories.Entities.Listing
                    {
                        ListingId = 1219,
                        StreetNumber = "182/88",
                        Street = "Southbank Boulevard",
                        Suburb = "Southbank",
                        State = "Victoria",
                        Postcode = 3006,
                        CategoryType = Enums.CategoryType.Residential,
                        StatusType = Enums.StatusType.Current,
                        DisplayPrice = "$600,000 - $618,000",
                        Title = "Rejuvenated Spacious North-Facing Retreat with Sweeping Panorama"
                    }, new Repositories.Entities.Listing
                    {
                        ListingId = 1358,
                        StreetNumber = "2307/151",
                        Street = "City Road",
                        Suburb = "Southbank",
                        State = "Victoria",
                        Postcode = 3006,
                        CategoryType = Enums.CategoryType.Rental,
                        StatusType = Enums.StatusType.Current,
                        DisplayPrice = "$400",
                        Title = "Southbank Grand: Gorgeous and Modern One Bedroom Apartment!"
                    }
            };
        }

    }
}
