using BizCover.Blaze.Infrastructure.Bus.Sample.Publisher.Publisher;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BizCover.Blaze.Infrastructure.Bus.Sample.Publisher.Models;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusPublisherController : ControllerBase
    {
        private readonly IOrderEventPublisher _orderEventPublisher;

        public BusPublisherController(IOrderEventPublisher orderEventPublisher)
        {
            _orderEventPublisher = orderEventPublisher;
        }

        [HttpPost]
        [Route("publish")]
        public async Task<IActionResult> Publish([FromBody] OrderEventModel orderEvent)
        {
            var result = await _orderEventPublisher.PublishOrderAsync(orderEvent.ProductName, orderEvent.OrderId);
            
            //this task.delay is required so consumer can consume the message just for integration testing purpose.
            await Task.Delay(5000);
            if (result)
            {
                return Ok();
            }

            // As there are no concrete error just for testing this controller returns 400
            return BadRequest();
        }
    }
}
