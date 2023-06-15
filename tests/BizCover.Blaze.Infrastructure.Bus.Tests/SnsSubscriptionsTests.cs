using System;
using System.Linq;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Xunit;
using Amazon.SimpleNotificationService.Model;
using MassTransit;
using System.Threading.Tasks;
using BizCover.Blaze.Infrastructure.Bus.Sample.Event;

namespace BizCover.Blaze.Infrastructure.Bus.Tests
{
    public class RemoveSubscriptionsWhenNotConsumedTests
    {
        // use real types 
        [Theory, AutoData]
        public void given_subscriptions_when_in_endpoint_and_consumed_with_type_having_consumer_added_then_ignored(List<Subscription> subscriptions, string topic)
        {
            subscriptions.ForEach(x => x.Endpoint = topic);
            subscriptions.ForEach(x => x.Protocol = "sqs");

            var types = new[] { typeof(OrderEventConsumer) };
            subscriptions = subscriptions.Zip(types).Select(tuple => { tuple.First.TopicArn += nameof(OrderEvent); return tuple.First; }).ToList();
            var snsSubscriptions = new SnsSubscriptions().FindSubscriptionsWithNoConsumers(subscriptions, types, topic);
            Assert.Empty(snsSubscriptions);
        }

        [Theory, AutoData]
        public void given_subscriptions_when_in_endpoint_and_consumed_with_multiple_types_having_single_consumer_then_ignored()
        {
            var queueName = "dev-au-blaze-documents";
            var subscriptions = new List<Subscription>
            {
                new Subscription
                {
                    Protocol = "sqs",
                    Endpoint = queueName,
                    TopicArn = $"arn:aws:sns:ap-southeast-2:156174591996:dev-au-blaze-Policies-{nameof(OrderEvent)}"
                },
                new Subscription
                {
                    Protocol = "sqs",
                    Endpoint = queueName,
                    TopicArn = $"arn:aws:sns:ap-southeast-2:156174591996:dev-au-blaze-Policies-{nameof(AcceptOrderCommand)}"
                }
            };
            var types = new[] { typeof(MultipleConsumer) };

            var snsSubscriptions = new SnsSubscriptions().FindSubscriptionsWithNoConsumers(subscriptions, types, queueName);
            Assert.Empty(snsSubscriptions);
        }

        [Theory, AutoData]
        public void given_subscriptions_when_in_endpoint_and_consumed_has_different_consumer_name_then_ignored()
        {
            var queueName = "dev-au-blaze-documents";
            var subscriptions = new List<Subscription>
            {
                new Subscription
                {
                    Protocol = "sqs",
                    Endpoint = queueName,
                    TopicArn = $"arn:aws:sns:ap-southeast-2:156174591996:dev-au-blaze-Policies-{nameof(OrderEvent)}"
                }
            };
            var types = new[] { typeof(DifferentConsumerName) };

            var snsSubscriptions = new SnsSubscriptions().FindSubscriptionsWithNoConsumers(subscriptions, types, queueName);
            Assert.Empty(snsSubscriptions);
        }

        [Theory, AutoData]
        public void given_subscriptions_when_in_endpoint_and_not_consumed_then_returned(List<Subscription> subscriptions, string topic, Type[] types)
        {
            subscriptions.ForEach(x => x.Endpoint = topic);
            subscriptions.ForEach(x => x.Protocol = "sqs");
            var snsSubscriptions = new SnsSubscriptions().FindSubscriptionsWithNoConsumers(subscriptions, types, topic);
            Assert.Equal(3, snsSubscriptions.Count());
        }

        [Theory, AutoData]
        public void given_non_sqs_subscriptions_when_in_endpoint_and_consumed_then_ignored(List<Subscription> subscriptions, string topic, Type[] types)
        {
            subscriptions.ForEach(x => x.Endpoint = topic);
            var snsSubscriptions = new SnsSubscriptions().FindSubscriptionsWithNoConsumers(subscriptions, types, topic);
            Assert.Empty(snsSubscriptions);
        }
    }

    public class OrderEventConsumer : IConsumer<OrderEvent>
    {
        public Task Consume(ConsumeContext<OrderEvent> context)
        {
            throw new NotImplementedException();
        }
    }

    public class MultipleConsumer : IConsumer<OrderEvent>, IConsumer<AcceptOrderCommand>
    {
        public Task Consume(ConsumeContext<OrderEvent> context)
        {
            throw new NotImplementedException();
        }

        public Task Consume(ConsumeContext<AcceptOrderCommand> context)
        {
            throw new NotImplementedException();
        }
    }

    public class DifferentConsumerName : IConsumer<OrderEvent>
    {
        public Task Consume(ConsumeContext<OrderEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
