using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly IGetListingsService _getListingsService;
        private readonly ILogger<ListingsController> _logger;

        public ListingsController(IGetListingsService getListingsService, ILogger<ListingsController> logger)
        {
            _getListingsService = getListingsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetListingsAsync([FromQuery] GetListingsRequest request)
        {
            _logger.LogInformation($"Getting Listing for payload : {JsonConvert.SerializeObject(request)}");

            var response = await _getListingsService.ExecuteAsync(request);
            return Ok(response);
        }
    }
}
