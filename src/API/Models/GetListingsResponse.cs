using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class GetListingsResponse
    {
        public IEnumerable<Item> Items { get; set; }
        public int Total { get; set; }
    }
}
