using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BizCover.Blaze.Infrastructure.Bus.Observers
{
    public class SendObserver : ISendObserver
    {
        private readonly ILogger<SendObserver> _logger;

        public SendObserver(ILogger<SendObserver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// called just before a message is sent, all the headers should be setup and everything
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PreSend<T>(SendContext<T> context) where T : class
        {
            _logger.LogDebug($"Pre-Send {typeof(T).Name} from {context.DestinationAddress} " +
                             $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// called just after a message it sent to the transport and acknowledged (i.e SQS, RabbitMQ)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PostSend<T>(SendContext<T> context) where T : class
        {
            _logger.LogDebug($"Post-Send {typeof(T).Name} from {context.DestinationAddress} " +
                             $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// called if an exception occurred sending the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception, $"Error while sending {typeof(T).Name} from {context.DestinationAddress} " +
                                        $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }
    }
}