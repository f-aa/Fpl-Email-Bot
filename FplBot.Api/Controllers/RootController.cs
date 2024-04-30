using Microsoft.AspNetCore.Mvc;

namespace FplBot.Api.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController(FplJob job) : ControllerBase
    {
        private readonly FplJob job = job;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await this.job.Run(null, null);

            return this.Ok("Completed.");
        }
    }
}
