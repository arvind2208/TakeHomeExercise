using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;
using API.Repositories.Entities;

namespace API.Repositories
{
    public interface IListingRepository
    {
        Task<IEnumerable<Listing>> GetListingsAsync(GetListingsRequest request);
    }
}