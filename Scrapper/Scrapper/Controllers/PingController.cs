using Microsoft.AspNetCore.Mvc;

namespace Scrapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "pong";
        }
    }
}
