using Microsoft.AspNetCore.Mvc;

namespace SignalR.Web.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult Get()
        {
            return new ContentResult
            {
                Content = "<html><body>Good!</body></html>",
                ContentType = "text/html",
            };
        }
    }
}
