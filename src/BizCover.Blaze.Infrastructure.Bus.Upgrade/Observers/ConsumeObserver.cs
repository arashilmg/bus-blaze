using MassTransit;
using Microsoft.Extensions.Logging;

namespace BizCover.Blaze.Infrastructure.Bus.Upgrade.Observers
{
    public class ConsumeObserver : IConsumeObserver
    {
        public readonly ILogger<ConsumeObserver> _logger;

        /// <param name="logger"></param>
        public ConsumeObserver(ILogger<ConsumeObserver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// called before the consumer's Consume method is called
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            //TODO: Update this for serialize
            _logger.LogDebug($"Consuming {typeof(T).Name} from {context.DestinationAddress} " +
                                   $"with messageId: {context.MessageId} " +
                                   $"and payload {System.Text.Json.JsonSerializer.Serialize(context.Message)}");

            return Task.CompletedTask;
        }

        /// <summary>
        /// called after the consumer's Consume method is called
        /// if an exception was thrown, the ConsumeFault method is called instead
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            _logger.LogDebug($"Post-Consuming {typeof(T).Name} from {context.DestinationAddress} " +
                             $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// called if the consumer's Consume method throws an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception, $"Error while consuming {typeof(T).Name} from {context.DestinationAddress} " +
                                        $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }
    }
}