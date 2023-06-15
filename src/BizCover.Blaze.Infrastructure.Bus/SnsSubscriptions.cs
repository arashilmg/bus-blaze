using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using MassTransit;

[assembly: InternalsVisibleTo("BizCover.Blaze.Infrastructure.Bus.Tests")]

namespace BizCover.Blaze.Infrastructure.Bus
{
    internal class SnsSubscriptions
    {
        internal IEnumerable<Subscription> FindSubscriptionsWithNoConsumers(IEnumerable<Subscription> subscriptions, Type[] consumerTypes, string endPoint)
        {
            var subscriptionsForQueue = subscriptions
                .Where(x => x.Protocol.ToLower() == "sqs" && x.Endpoint.Contains(endPoint))
                .ToList();

            //Find the event name from the IConsumer<event_here>
            //E.g AcceptOrderCommandConsumer returns acceptedordercommand because IConsumer<AcceptOrderCommand>
            var eventNames = consumerTypes
                .SelectMany(consumerType => consumerType
                    .GetInterfaces()
                    .Where(@interface => @interface.Name
                        .Contains(typeof(IConsumer).Name))
                    .SelectMany(s => s.GenericTypeArguments)
                    .Select(s => s.Name
                        .ToLower()));

            return subscriptionsForQueue
                .Where(x => eventNames.Any(eventName => x.TopicArn.ToLower().EndsWith(eventName)) == false)
                .ToList();
        }

        internal Task[] RemoveIn1Minute(IEnumerable<string> subscriptionUri, ILogger logger, IAmazonSimpleNotificationService sns)
        {
            // fire and forget tasks for gradual / slow roll out.
            return subscriptionUri.Select(sub => Task.Run(async () =>
            {
                try
                {
                    logger.LogInformation($"Removing subscription {sub} as no consumer found");

                    await Task.Delay(TimeSpan.FromMinutes(1));

                    var response = await sns.UnsubscribeAsync(new UnsubscribeRequest(sub));

                    logger.LogInformation($"Response for subscription {sub} as no consumer found code {response.HttpStatusCode}");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"Error removing subscription {ex.Message}");
                }


            })).ToArray();
        }
    }
}
