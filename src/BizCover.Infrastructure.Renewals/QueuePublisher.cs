using BizCover.Application.Renewals.Helpers;
using BizCover.Blaze.Infrastructure.Bus.Internals;
using MassTransit;

namespace BizCover.Infrastructure.Renewals
{
    public class QueuePublisher : IQueuePublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IResolveEndpoint _resolveEndpoint;
        private readonly IPublishEndpoint _publishEndpoint;

        public QueuePublisher(ISendEndpointProvider sendEndpointProvider, IResolveEndpoint resolveEndpoint,
            IPublishEndpoint publishEndpoint)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _resolveEndpoint = resolveEndpoint;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Send<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            var endpoint = _resolveEndpoint.ResolveDefaultEndpointForSelf();
            var sender = await _sendEndpointProvider.GetSendEndpoint(endpoint);
            await sender.Send(command, cancellationToken);
        }

        public async Task Publish<TCommand>(TCommand command, CancellationToken cancellationToken) =>
            await _publishEndpoint.Publish(command, cancellationToken);
    }
}
