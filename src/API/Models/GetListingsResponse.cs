using System.Collections.Generic;

namespace API.Models
{
    public class GetListingsResponse
    {
        public IEnumerable<Item> Items { get; set; }
        public int Total { get; set; }
    }
}
