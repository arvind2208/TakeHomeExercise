using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using API.Repositories;
using API.Repositories.Entities;
using API.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API.Tests
{
    public class GetListingsServiceTests
    {
        private readonly Mock<IListingRepository> _mockListingRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ILogger<GetListingsService> _logger;

        public GetListingsServiceTests()
        {
            var mockLogger = new Mock<ILogger<GetListingsService>>();
            _logger = mockLogger.Object;

            _mockListingRepository = new Mock<IListingRepository>();

            _mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task WhenListingsExistInRepoOrCacheThenReturnsCorrectNoOfItemsAndCount()
        {
            _mockListingRepository.Setup(x => x.GetListingsAsync(It.IsAny<GetListingsRequest>()))
                .ReturnsAsync(GetDummyListings());

            var mapper = GetMockMapper();

            var service = new GetListingsService(_mockListingRepository.Object, mapper, _logger);

            var result = await service.ExecuteAsync(new Models.GetListingsRequest { Suburb = "southbank" });

            Assert.Equal(5, result.Items.Count());
            Assert.Equal(5, result.Total);
        }

        [Fact]
        public async Task WhenListingsRepoThrowsExceptionThensExceptionIsBubbledUp()
        {
            _mockListingRepository.Setup(x => x.GetListingsAsync(It.IsAny<GetListingsRequest>()))
                .ThrowsAsync(new Exception("some ex"));

            var mapper = GetMockMapper();

            var service = new GetListingsService(_mockListingRepository.Object, mapper, _logger);

            var exception = await Assert.ThrowsAsync<Exception>(() => service.ExecuteAsync(new Models.GetListingsRequest { Suburb = "southbank" }));

            Assert.Equal("some ex", exception.Message);
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

        private static IMapper GetMockMapper()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(MappingProfile));
            var serviceProvider = services.BuildServiceProvider();
            IMapper mapper = serviceProvider.GetService<IMapper>();
            return mapper;
        }
    }
}
