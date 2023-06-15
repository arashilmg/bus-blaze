using System;
using MassTransit;
using System.Threading.Tasks;
using BizCover.Blaze.Infrastructure.Bus.Sample.Event;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;

namespace BizCover.Blaze.Infrastructure.Bus.Sample.Publisher.Publisher
{
    public interface IOrderEventPublisher
    {
        Task<bool> PublishOrderAsync(string productName, string orderId);
    }
    public class OrderEventPublisher:IOrderEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderEventPublisher> _logger;

        public OrderEventPublisher(IPublishEndpoint publishEndpoint, ILogger<OrderEventPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> PublishOrderAsync(string productName, string orderId)
        {
            _logger.LogInformation($"Publishing OrderEvent for product {productName}");

            try
            {
                await _publishEndpoint.Publish<OrderEvent>(new {ProductName = productName, OrderId = orderId});

                _logger.LogInformation($"Published OrderEvent for product {productName} successfully");
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,$"Unable to publish OrderEvent for product {productName}");
                return false;
            }
        }
    }
}