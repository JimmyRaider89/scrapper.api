using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scrapper.Models;
using Scrapper.Scrappers;
using System.Threading.Tasks;

namespace Scrapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RankingController : ControllerBase
    {
        private readonly ILogger<RankingController> _logger;
        private readonly ScrapperFactory _scrapperFactory;

        public RankingController(ILogger<RankingController> logger, ScrapperFactory scrapperFactory)
        {
            _logger = logger;
            _scrapperFactory = scrapperFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Criteria criteria)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Bad Request");
                return BadRequest();
            }
            var result = await _scrapperFactory.GetScraper(criteria.Engine).ScrapeAsync(criteria.KeyWords);
            return Ok(result);
        }
    }
}
