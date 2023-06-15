using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace BizCover.Api.Renewals.ComponentTests.Fixtures;

public class SNSHelper
{
    private const string PolicyBoundEventTopicName = "local-au-blaze-Policies-PolicyBoundEvent";
    public string? TopicArn { get; private set; }

    public IAmazonSimpleNotificationService SnsClient { get; }

    public SNSHelper(string serviceUrl)
    {
        SnsClient = new AmazonSimpleNotificationServiceClient("ignore", "ignore", new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
    }

    public async Task Init()
    {
        if(TopicArn !=  null)
            return;

        var response = await SnsClient.CreateTopicAsync(new CreateTopicRequest { Name = PolicyBoundEventTopicName });
        TopicArn = response.TopicArn;
    }
}
