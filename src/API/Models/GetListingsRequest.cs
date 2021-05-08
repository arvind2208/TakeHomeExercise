using API.Enums;

namespace API.Models
{
    public class GetListingsRequest
    {
        public string Suburb { get; set; }
        public CategoryType? CategoryType { get; set; }
        public StatusType? StatusType { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
