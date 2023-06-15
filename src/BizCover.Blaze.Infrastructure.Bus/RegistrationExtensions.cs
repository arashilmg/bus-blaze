using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using GreenPipes;
using MassTransit;
using MassTransit.AmazonSqsTransport;
using BizCover.Blaze.Infrastructure.Bus.BlazeSNSConfigurator;
using BizCover.Blaze.Infrastructure.Bus.Internals;
using BizCover.Blaze.Infrastructure.Bus.Observers;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace BizCover.Blaze.Infrastructure.Bus
{
    public static class RegistrationExtensions
    {
        private static Task[] subscriptionRemovalTasks;
        private static Task deadLetterQueueAttributesUpdateTask;

        public static IServiceCollection AddBlazeBus(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            Action<IAmazonSqsReceiveEndpointConfigurator> subscriptions = null,
            Action<IReceiveEndpointConfigurator, IRegistration> configureConsumers = null,           
            params Type[] consumerTypes)
        {
            return serviceCollection.AddBlazeBus(configuration, subscriptions, configureConsumers, null, consumerTypes);
        }

            public static IServiceCollection AddBlazeBus(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            Action<IAmazonSqsReceiveEndpointConfigurator> subscriptions = null,
            Action<IReceiveEndpointConfigurator, IRegistration> configureConsumers = null,
            Action<IBlazeSNSConfigurator> topicWithoutConsmers = null,
            params Type[] consumerTypes)
        {
            var awsOptions = configuration.GetAWSOptions();
            var busOptions = configuration.GetBusOptions();

            var entityNameFormatter = new BizCoverEntityNameFormatter(busOptions);

            serviceCollection.AddRequiredServicesForBlazeBus(awsOptions);

            serviceCollection.AddMassTransit(bus =>
            {
                if (consumerTypes.Any())  bus.AddConsumers(consumerTypes);             

                bus.AddAmazonSqsMessageScheduler();

                bus.UsingAmazonSqs((busRegistrationContext, amazonSqsBusFactoryConfigurator) =>
                {
                    amazonSqsBusFactoryConfigurator.ConnectObservers(busRegistrationContext);

                    ILogger logger = busRegistrationContext.GetRequiredService<ILoggerFactory>().CreateLogger("AddBlazeBus");

                    amazonSqsBusFactoryConfigurator.MessageTopology.SetEntityNameFormatter(entityNameFormatter);
                    var sqs = busRegistrationContext.GetRequiredService<IAmazonSQS>();
                    var sns = busRegistrationContext.GetRequiredService<IAmazonSimpleNotificationService>();

                    amazonSqsBusFactoryConfigurator.Host(awsOptions.Region.SystemName, amazonSqsHostConfigurator =>
                    {
                        logger.LogInformation($"SQS URL:{sqs.Config.DetermineServiceURL()}");
                        amazonSqsHostConfigurator.Config(new AmazonSQSConfig { ServiceURL = sqs.Config.DetermineServiceURL() });
                        logger.LogInformation($"SNS URL:{sns.Config.DetermineServiceURL()}");
                        amazonSqsHostConfigurator.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = sns.Config.DetermineServiceURL() });
                    });

                    deadLetterQueueAttributesUpdateTask = UpdateDeadLetterQueueAttributes(sqs, busOptions, logger);

                    if (subscriptions != null || configureConsumers != null)
                    {
                        amazonSqsBusFactoryConfigurator.ReceiveEndpoint(
                            busOptions.QueuePrefix.ToAwsQueueName(), c =>
                            {
                                subscriptions?.Invoke(c);
                                c.UseMessageRetry(r => r.Intervals(500, 1000, 1500));
                                configureConsumers?.Invoke(c, busRegistrationContext);
                            });

                        var existingSubscriptions = sns
                           .ListSubscriptionsAsync()
                           .Result;
                        
                        if (existingSubscriptions.Subscriptions.Any())
                        {
                            try
                            {
                                var subscriptionsRetrieved = existingSubscriptions.Subscriptions.ToList();
                                while (existingSubscriptions.NextToken != null)
                                {
                                    existingSubscriptions = sns
                                        .ListSubscriptionsAsync(existingSubscriptions.NextToken)
                                        .Result;
                                    subscriptionsRetrieved.AddRange(existingSubscriptions.Subscriptions);
                                }

                                var snsSubscriptions = new SnsSubscriptions();
                                var subsToRemove = snsSubscriptions.FindSubscriptionsWithNoConsumers(subscriptionsRetrieved, consumerTypes, busOptions.QueuePrefix);

                                subscriptionRemovalTasks = snsSubscriptions.RemoveIn1Minute(subsToRemove.Select(x => x.SubscriptionArn), logger, sns);
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning(ex, $"error with subscription removal error, will try again on next service restart {ex.Message}");
                            }
                        }
                    }

                    var blazeSnsConfigurator = busRegistrationContext.GetRequiredService<IBlazeSNSConfigurator>();
                    blazeSnsConfigurator.EntityNameFormatter = entityNameFormatter;
                    topicWithoutConsmers?.Invoke(blazeSnsConfigurator);

                    amazonSqsBusFactoryConfigurator.ConfigureJsonSerializer(settings =>
                    {
                        settings.DefaultValueHandling = DefaultValueHandling.Include;
                        return settings;
                    });
                });

            }).AddMassTransitHostedService(true);

            return serviceCollection;
        } 

        private static void AddRequiredServicesForBlazeBus(this IServiceCollection serviceCollection, AWSOptions awsOptions)
             => serviceCollection.AddDefaultAWSOptions(awsOptions)
                 .AddAWSService<IAmazonSQS>()
                 .AddAWSService<IAmazonSimpleNotificationService>()
                 .AddSingleton<IPublishObserver, PublishObserver>()
                 .AddSingleton<ISendObserver, SendObserver>()
                 .AddSingleton<IConsumeObserver, ConsumeObserver>()
                 .AddSingleton<IReceiveObserver, ReceiveObserver>()
                 .AddSingleton<IResolveEndpoint, ResolveEndPoint>()
                 .AddSingleton<IBlazeSNSConfigurator, BlazeSNSConfigurator.BlazeSNSConfigurator>();

        private static void ConnectObservers(this IAmazonSqsBusFactoryConfigurator amazonSqsBusFactoryConfigurator,
            IBusRegistrationContext busRegistrationContext)
        {
            amazonSqsBusFactoryConfigurator.ConnectPublishObserver(busRegistrationContext.GetRequiredService<IPublishObserver>());

            amazonSqsBusFactoryConfigurator.ConnectConsumeObserver(busRegistrationContext.GetRequiredService<IConsumeObserver>());

            amazonSqsBusFactoryConfigurator.ConnectSendObserver(busRegistrationContext.GetRequiredService<ISendObserver>());

            amazonSqsBusFactoryConfigurator.ConnectReceiveObserver(busRegistrationContext.GetRequiredService<IReceiveObserver>());
        }

        private static Task UpdateDeadLetterQueueAttributes(IAmazonSQS sqs, BusOptions busOptions, ILogger logger)
        {
            // fire and forget tasks for gradual / slow roll out.
            return Task.Run(async () =>
            {
                try
                {
                    var getQueueUrlResponse = await GetQueueUrlAsync(sqs, busOptions);
                    if (getQueueUrlResponse == null)
                    {
                        logger.LogInformation("Deadletter queue does not exist yet. No attributes to update");
                        return;
                    }

                    var getQueueAttributesResponse = await sqs.GetQueueAttributesAsync(getQueueUrlResponse.QueueUrl, null);
                    
                    //Keep message in queue for 14 days instead of 4 days by default
                    getQueueAttributesResponse.Attributes["MessageRetentionPeriod"] = "1209600";

                    var SetQueueAttributesResponse = await sqs.SetQueueAttributesAsync(new SetQueueAttributesRequest
                    {
                        QueueUrl = getQueueUrlResponse.QueueUrl,
                        Attributes = getQueueAttributesResponse.Attributes,
                    });

                    logger.LogInformation($"Response for updating deadletter queue attributes: Http code {SetQueueAttributesResponse.HttpStatusCode}");
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, $"Error updating deadletter queue attribute {exception.Message}");
                }
            });
        }

        private static async Task<GetQueueUrlResponse> GetQueueUrlAsync(IAmazonSQS sqs, BusOptions busOptions)
        {
            try
            {
                return await sqs.GetQueueUrlAsync(busOptions.QueuePrefix.ToAwsDeadLetterQueueName());
            }
            catch (QueueDoesNotExistException exception)
            {
                return null;
            }
        }
    }
}
