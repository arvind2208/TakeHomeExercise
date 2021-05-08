using API.Models;
using API.Repositories.Entities;
using API.Services;
using Xunit;

namespace API.Tests
{
	public class ShortPriceResolverTests
	{
		[Theory]
        [InlineData("$580,000", "$580k")]
        [InlineData("$610", "$610")]
        [InlineData("PROPERTY LAUNCH", "PROPERTY LAUNCH")]
        [InlineData("Offers Over $735,000", "Offers Over $735k")]
        [InlineData("$428,500", "$428.5k")]
        [InlineData("$428,509", "$428.51k")]
        [InlineData("$80,000", "$80k")]
        [InlineData("$399,990", "$399.99k")]
        [InlineData("$1,500,000", "$1.5m")]
        [InlineData("For Sale", "")]
        [InlineData("$445,000-$465,000", "$445k-$465k")]
        [InlineData("$269,000 - $279,000 | SELLING THIS WEEKEND.", "$269k - $279k | SELLING THIS WEEKEND.")]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void WhenPriceExistsInDisplayPriceThenItIsShortendedWherePossible(string input, string expected)
		{
            //Assign
            var source = new Listing
            {
                DisplayPrice = input
            };

            var destination = new Item
            {
                ShortPrice = ""
            };
 
            var resolver = new ShortPriceResolver();

            //Act
            var actual = resolver.Resolve(source, destination, null, null);

            //Assert
            Assert.Equal(expected, actual);
        }
	}
}
