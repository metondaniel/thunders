using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.Domain;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.WebAPI.Controllers
{
    [ApiController]
    [Route("api/toll")]
    public class TollController : ControllerBase
    {
        private readonly IMessageSender _messageSender;

        public TollController(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [HttpPost("usage")]
        public async Task<IActionResult> PostUsage([FromBody] TollUsage usage)
        {
            await _messageSender.Publish(usage);
            return Accepted();
        }
    }
}