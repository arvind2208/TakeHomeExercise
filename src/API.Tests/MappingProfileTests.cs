using System;
using API.Models;
using API.Repositories.Entities;
using API.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API.Tests
{
    public class MappingProfileTests
    {
        [Fact]
        public void MappingsAreValid()
        {
            var mappingProfile = new MappingProfile();

            var config = new MapperConfiguration(mappingProfile);
            var mapper = new Mapper(config);

            ((IMapper)mapper).ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData("214/19", "Pentridge Boulevard","Coburg","Victoria",3058, "214/19 Pentridge Boulevard Coburg Victoria 3058")]
        [InlineData("214/19", null, "Coburg", "Victoria", 3058, "214/19 Coburg Victoria 3058")]
        [InlineData("214/19", null, null, "Victoria", 3058, "214/19 Victoria 3058")]
        [InlineData("214/19", null, null, null, 3058, "214/19 3058")]
        [InlineData("214/19", null, null, null, null, "214/19")]
        [InlineData(null, null, null, null, null, "")]
        [InlineData("", "", "", "", null, "")]
        public void WhenIndividualAddressFieldsExistsThenConcatenatedStringIsReturned(string stNo, string st, string suburb, string state, int? postcode, string expected)
        {
            //Arrange
            var listing = new Listing
            {
                StreetNumber = stNo,
                Street = st,
                Suburb = suburb,
                State = state,
                Postcode = postcode,
                CategoryType = Enums.CategoryType.Residential,
                StatusType = Enums.StatusType.Current,
                DisplayPrice = "$695,000 - $760,000",
                Title = "Modern Apartment",
                ListingId = 1488
            };

            var mapper = GetMockMapper();

            //Act
            var item = mapper.Map<Item>(listing);

            //Assign
            Assert.Equal(expected, item.Address);
        }

        [Fact]
        public void WhenValidListingWithAllFieldsThenResponseIsMappedCorrectly()
        {
            //Arrange
            var listing = new Listing
            {
                StreetNumber = "214/19",
                Street = "Pentridge Boulevard",
                Suburb = "Coburg",
                State = "Victoria",
                Postcode = 3058,
                CategoryType = Enums.CategoryType.Residential,
                StatusType = Enums.StatusType.Current,
                DisplayPrice = "$695,000 - $760,000",
                Title = "Modern Apartment",
                ListingId = 1488
            };

            var mapper = GetMockMapper();

            //Act
            var item = mapper.Map<Item>(listing);

            //Assign
            Assert.Equal("214/19 Pentridge Boulevard Coburg Victoria 3058", item.Address);
            Assert.Equal(Enums.CategoryType.Residential.ToString(), item.CategoryType);
            Assert.Equal(Enums.StatusType.Current.ToString(), item.StatusType);
            Assert.Equal("$695,000 - $760,000", item.DisplayPrice);
            Assert.Equal("$695k - $760k", item.ShortPrice);
            Assert.Equal("Modern Apartment", item.Title);
            Assert.Equal(1488, item.ListingId);
        }

        [Fact]
        public void WhenFieldsAreMissingThenDoesNotThrowException()
        {
            //Arrange
            var listing = new Listing
            {
            };

            var mapper = GetMockMapper();

            //Act
            var item = mapper.Map<Item>(listing);

            //Assign
            Assert.Equal("", item.Address);
            Assert.Equal("0", item.CategoryType);
            Assert.Equal("0", item.StatusType);
            Assert.Null(item.DisplayPrice);
            Assert.Null(item.ShortPrice);
            Assert.Null(item.Title);
            Assert.Equal(0, item.ListingId);
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
