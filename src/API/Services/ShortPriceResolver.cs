using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Models;
using API.Repositories.Entities;
using AutoMapper;

namespace API.Services
{
    public class ShortPriceResolver : IValueResolver<Listing, Item, string>
    {
        public string Resolve(Listing source, Item destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.DisplayPrice))
                return null;

            string shortPrice = source.DisplayPrice;

            var pattern = @"(\d,{0,4})+";

            Regex displayPriceRegex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection displayPriceMatches = displayPriceRegex.Matches(source.DisplayPrice);

            foreach (Match m in displayPriceMatches)
            {
                var displayPrice = m.Value;

                if (decimal.TryParse(displayPrice.Replace(",", string.Empty), out var dp))
                {
                    if (dp >= 1000000)
                    {
                        shortPrice = ConvertToShortPrice(shortPrice, displayPrice, dp, 1000000, "m");
                    }

                    if (dp >= 1000)
                    {
                        shortPrice = ConvertToShortPrice(shortPrice, displayPrice, dp, 1000, "k");
                    }
                }
            }

            if (shortPrice.Equals("for sale", StringComparison.OrdinalIgnoreCase))
                shortPrice = string.Empty;

            return shortPrice;
        }

        private string ConvertToShortPrice(string shortPrice, string displayPrice, decimal dp, long numberGroup, string suffix)
        {
            var spNum = dp / numberGroup;

            var val = spNum % 1 == 0 ? 
                spNum : Math.Round(spNum, 2);

            var sp = $"{val}{suffix}";

            shortPrice = shortPrice.Replace(displayPrice, sp);
            return shortPrice;
        }
    }
}
