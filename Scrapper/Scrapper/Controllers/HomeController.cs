using Microsoft.AspNetCore.Mvc;

namespace Scrapper.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Welcome to the Search Engine Scrapper service";
        }
    }
}
