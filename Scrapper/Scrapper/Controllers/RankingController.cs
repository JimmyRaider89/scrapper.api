using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scrapper.Models;
using Scrapper.Scrappers;
using System.Threading.Tasks;

namespace Scrapper.Controllers
{
    [ApiController]
    [EnableCors("SiteCorsPolicy")]
    [Route("[controller]")]
    public class RankingController : ControllerBase
    {
        private readonly ScrapperFactory _scrapperFactory;

        public RankingController(ScrapperFactory scrapperFactory)
        {
            _scrapperFactory = scrapperFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Criteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _scrapperFactory.GetScraper(criteria.Engine).ScrapeAsync(criteria.KeyWords);
            return Ok(result);
        }
    }
}
