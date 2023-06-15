using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using MassTransit;
using MassTransit.AmazonSqsTransport;
using Microsoft.Extensions.Logging;

namespace BizCover.Blaze.Infrastructure.Bus.Upgrade.BlazeSNSConfigurator
{
    public interface IBlazeSNSConfigurator
    {
        Task ConfigureSNS<T>() where T : class;
        IEntityNameFormatter EntityNameFormatter { get; set; }
    }
    public class BlazeSNSConfigurator : IBlazeSNSConfigurator
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly ILogger<BlazeSNSConfigurator> _logger;

        public BlazeSNSConfigurator(IAmazonSimpleNotificationService amazonSimpleNotificationService, ILogger<BlazeSNSConfigurator> logger)
        {
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            _logger = logger;
        }

        public IEntityNameFormatter EntityNameFormatter { get; set; }

        public async Task ConfigureSNS<T>() where T : class
        {
            var topicName = EntityNameFormatter.FormatEntityName<T>();
            var createTopicRequest = new CreateTopicRequest
            {
                Name = topicName
            };
            var response = await _amazonSimpleNotificationService.CreateTopicAsync(createTopicRequest).ConfigureAwait(false);
            response.EnsureSuccessfulResponse();
            _logger.LogInformation($"Created Topic {topicName} with Topic ARN {response.TopicArn} from BlazeConfigurator");
        }
    }
}
