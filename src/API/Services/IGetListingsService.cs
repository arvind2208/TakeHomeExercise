using System.Threading.Tasks;
using API.Models;

namespace API.Services
{
    public interface IGetListingsService
    {
        Task<GetListingsResponse> ExecuteAsync(GetListingsRequest request);
    }
}