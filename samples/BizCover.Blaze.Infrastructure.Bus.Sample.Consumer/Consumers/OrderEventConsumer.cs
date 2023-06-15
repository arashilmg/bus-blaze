using System;
using System.Threading.Tasks;

using MassTransit;

using BizCover.Blaze.Infrastructure.Bus.Sample.Event;
using BizCover.Blaze.Infrastructure.Bus.Upgrade.Internals;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Consumer.Consumers
{
    public class OrderEventConsumer : IConsumer<OrderEvent>
    {
        private readonly IResolveEndpoint _resolveEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderEventConsumer(IResolveEndpoint resolveEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _resolveEndpoint = resolveEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderEvent> context)
        {
            var endpointUri = _resolveEndpoint.ResolveDefaultEndpointForSelf();
            var sender = await _sendEndpointProvider.GetSendEndpoint(endpointUri);
            await sender.Send(
                new AcceptOrderCommand
                {
                    ProductName = context.Message.ProductName,
                    OrderId = context.Message.OrderId,
                    OrderedAcceptedId = Guid.NewGuid().ToString()
                });
        }
    }
}