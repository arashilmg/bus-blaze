using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[assembly: Xunit.AssemblyTrait("Type", "Component")]
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = false)]
namespace BizCover.Api.Renewals.ComponentTests.Fixtures;

public class ComponentTestFixture
{
    private readonly IConfiguration _config;

    public SNSHelper SNSHelper { get; private set; }
    public string SvcHttpUrl => _config["SvcHttpUrl"];
    public string SvcGrpcUrl => _config["SvcGrpcUrl"];
    public string ServiceUrl => _config["AWS:ServiceURL"];

    public ComponentTestFixture()
    {       
        var configDefaults = new Dictionary<string, string>
        {
            {"SvcHttpUrl", "http://localhost:8230/"},
            {"SvcGrpcUrl", "http://localhost:8231/"},
            {"AWS:ServiceURL", "http://localhost:4566" },
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDefaults)
            .AddEnvironmentVariables()
            .Build();

        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

        SNSHelper = new SNSHelper(ServiceUrl);
    }

    public async Task PublishPolicyBoundEvent(dynamic policyBoundEvent) =>
        await PublishEvent(policyBoundEvent, "urn:message:BizCover.Messages.Policies:PolicyBoundEvent");

    public async Task PublishOrderCompletedEvent(dynamic orderCompletedEvent) =>
        await PublishEvent(orderCompletedEvent, "urn:message:BizCover.Messages.Orders:OrderCompletedEvent");

    public async Task PublishMigratePolicyCommand(dynamic migratePolicyCommand) =>
        await PublishEvent(migratePolicyCommand, "urn:message:BizCover.Messages.Renewals:MigratePolicyCommand");

    public async Task PublishStatusChangeEvent(dynamic statusChange) =>
        await PublishEvent(statusChange, "urn:message:BizCover.Messages.Scheduler:StatusChange");

    public async Task PublishGenerateRenewalOrderCommand(dynamic renewalOrderCommand) =>
        await PublishEvent(renewalOrderCommand, "urn:message:BizCover.Messages.Renewals:GenerateRenewalOrderCommand");

    private async Task PublishEvent(dynamic eventMessage, string eventMessageType)
    {
        await SNSHelper.Init();
        var eventBody = new
        {
            messageType = new[] { eventMessageType },
            message = eventMessage
        };
        
        // publish
        await SNSHelper.SnsClient.PublishAsync(SNSHelper.TopicArn, JsonConvert.SerializeObject(eventBody));
        
        // wait for the message to be consumed
        Thread.Sleep(3000);
    }
}
