using BizCover.Blaze.Infrastructure.Bus.Sample.Event;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Consumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusReceiverController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<BusReceiverController> _logger;
        public BusReceiverController( IMemoryCache memoryCache, ILogger<BusReceiverController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetFromConsumer([FromRoute] string id)
        {
            var result = _memoryCache.TryGetValue(id, out string acceptOrderCommandJson);
            if (result)
            {
                var acceptOrderCommand = System.Text.Json.JsonSerializer.Deserialize<AcceptOrderCommand>(acceptOrderCommandJson);
                return Ok(acceptOrderCommand.OrderedAcceptedId);
            }

            _logger.LogInformation("finished");
            //As there are no concrete error just for testing this controller returns 400
            return BadRequest();
        }
    }
}
