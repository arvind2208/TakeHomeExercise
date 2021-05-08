using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class GetListingsService : IGetListingsService
    {
        private readonly IListingRepository _listingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetListingsService> _logger;
        public GetListingsService(IListingRepository listingRepository, IMapper mapper,
            ILogger<GetListingsService> logger)
        {
            _listingRepository = listingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetListingsResponse> ExecuteAsync(GetListingsRequest request)
        {
            var listings = await _listingRepository.GetListingsAsync(request);

            _logger.LogInformation("Mapping entities to response");
            var items = _mapper.Map<IEnumerable<Item>>(listings);

            _logger.LogInformation("Get listings successful");

            return new GetListingsResponse
            {
                Items = items,
                Total = items.Count()
            };
        }
    }
}
