using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BizCover.Blaze.Infrastructure.Bus.Observers
{
    public class PublishObserver : IPublishObserver
    {
        public readonly ILogger<PublishObserver> _logger;

        public PublishObserver(ILogger<PublishObserver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// called right before the message is published (sent to exchange or topic)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            _logger.LogDebug($"Pre-Publishing {typeof(T).Name} to {context.DestinationAddress} " +
                             $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// called after the message is published (and acked by the broker if RabbitMQ)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            _logger.LogDebug($"Published {typeof(T).Name} to {context.DestinationAddress} " +
                                   $"with messageId: {context.MessageId} " +
                                   $"and payload {JsonConvert.SerializeObject(context, Formatting.Indented)}");

            return Task.CompletedTask;
        }

        /// <summary>
        /// called if there was an exception publishing the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception, $"Error while publishing {typeof(T).Name} to {context.DestinationAddress} " +
                             $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }
    }
}