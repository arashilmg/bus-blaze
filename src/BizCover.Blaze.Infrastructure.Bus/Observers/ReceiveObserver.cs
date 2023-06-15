using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BizCover.Blaze.Infrastructure.Bus.Internals;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BizCover.Blaze.Infrastructure.Bus.Observers
{
    public class ReceiveObserver : IReceiveObserver
    {
        public readonly ILogger<ReceiveObserver> _logger;

        public ReceiveObserver(ILogger<ReceiveObserver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///  called immediately after the message was delivery by the transport
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PreReceive(ReceiveContext context)
        {
            _logger.LogDebug($"Pre-Receive body : {context.GetBody().ConvertToString()} " +
                             $"with messageId: {context.GetMessageId()}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// called after the message has been received and processed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PostReceive(ReceiveContext context)
        {
            _logger.LogDebug($"Post-Receive body : {context.GetBody().ConvertToString()} " +
                             $"with messageId: {context.GetMessageId()}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// called when the message was consumed, once for each consumer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <returns></returns>
        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            _logger.LogInformation("Post-Message-consuming {EventName} from {DestinationAddress} with messageId: {MessageId} and time {messageElapsedMilliseconds}",
                typeof(T).Name, context.DestinationAddress, context.MessageId, duration.Milliseconds);

            return Task.CompletedTask;
        }

        /// <summary>
        /// called when the message is consumed but the consumer throws an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            _logger.LogError("Message {EventName} consumed by {ConsumerType} in {Elapsed:0.0000} ms", typeof(T).Name, consumerType, duration.Milliseconds);

            //this wont be called as we have consumeobserver implemented
            _logger.LogError(exception, $"Error while consuming {typeof(T).Name} from {context.DestinationAddress} " +
                                        $"with messageId: {context.MessageId}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// called when an exception occurs early in the message processing, such as deserialization, etc.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            _logger.LogError(exception, $"Error while Receiving message {context.GetBody().ConvertToString()} " +
                                        $"with messageId: {context.GetMessageId()}");
            return Task.CompletedTask;
        }
    }
}