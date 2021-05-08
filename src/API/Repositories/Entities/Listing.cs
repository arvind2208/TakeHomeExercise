using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.Repositories.Entities
{
    public class Listing
    {
        public int ListingId { get; set; }
        public string StreetNumber { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public int Postcode { get; set; }
        public CategoryType CategoryType { get; set; }
        public StatusType StatusType { get; set; }
        public string DisplayPrice { get; set; }
        public string Title { get; set; }
    }
}
