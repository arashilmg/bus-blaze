using BizCover.Blaze.Infrastructure.Bus.Sample.Event;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;


namespace BizCover.Blaze.Infrastructure.Bus.Sample.Consumer.Consumers
{
    public class AcceptOrderCommandConsumer : IConsumer<AcceptOrderCommand>
    {
        private readonly IMemoryCache _memoryCache;

        public AcceptOrderCommandConsumer(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

       public Task Consume(ConsumeContext<AcceptOrderCommand> context)
       {
           var acceptOrderCommand = new AcceptOrderCommand()
           {
               ProductName = context.Message.ProductName,
               OrderId = context.Message.OrderId,
               OrderedAcceptedId = context.Message.OrderedAcceptedId
           };

            _memoryCache.Set(context.Message.OrderId,System.Text.Json.JsonSerializer.Serialize(acceptOrderCommand));

            return Task.CompletedTask;
       }
    }
}