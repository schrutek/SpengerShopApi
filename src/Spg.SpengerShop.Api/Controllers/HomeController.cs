using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Spg.SpengerShop.Api.Controllers
{
    [Route("api/Home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public OkObjectResult Index()
        {
            return new OkObjectResult("Welcome!");
        }
    }
}
