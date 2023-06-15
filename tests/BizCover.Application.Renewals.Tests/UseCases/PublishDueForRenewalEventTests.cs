using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Entity.Renewals;
using BizCover.Messages.Renewals;
using Moq;
using Xunit;

namespace BizCover.Application.Renewals.Tests.UseCases;

public class PublishDueForRenewalEventTests
{
    private readonly FakeRepository<Renewal> _fakeRepository = new();
    private readonly Mock<IQueuePublisher> _mockQueuePublisher = new();
    private readonly Mock<IRenewalConfigService> _mockRenewalConfigService = new();

    private readonly PublishDueForRenewalEvents _publishDueForRenewalEvents;

    public PublishDueForRenewalEventTests()
    {
        _mockRenewalConfigService.Setup(x => x.CanAutoRenew(It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(() => true);

        _publishDueForRenewalEvents = new PublishDueForRenewalEvents(_fakeRepository, _mockQueuePublisher.Object, _mockRenewalConfigService.Object);
    }

    [Fact]
    public async Task Run_Should_Only_Publish_Valid_Events_When_Executed()
    {
        var expiringPolicyId = Guid.NewGuid();
        _fakeRepository.Entities = GetRenewals(expiringPolicyId);

        _mockQueuePublisher.Setup(x => x.Publish(It.IsAny<DueForRenewalEvent>(), CancellationToken.None));

        await _publishDueForRenewalEvents.Run(CancellationToken.None);

        _mockQueuePublisher.Verify(x => x.Publish(It.IsAny<DueForRenewalEvent>(), CancellationToken.None),
            Times.Exactly(2));

    }

    private static IEnumerable<Renewal> GetRenewals(Guid expiringPolicyId)
    {
        var issuedPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Issued
        };

        var activePolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date, Initiated = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Active
        };

        var cancelledPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date, Initiated = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Cancelled
        };

        var expiredPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = Guid.NewGuid(),
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date, Initiated = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Expired
        };

        var orderGeneratedPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates
            {
                Initiation = DateTime.UtcNow.Date,
                Initiated = DateTime.UtcNow.Date,
                OrderGeneration = DateTime.UtcNow.Date,
                OrderGenerated = DateTime.UtcNow.Date
            },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Active
        };

        var policyPreviouslyRenewed = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            RenewedPolicyId = Guid.NewGuid()
        };

        return new List<Renewal>
        {
            issuedPolicyRenewal,                // do not send
            activePolicyRenewal,                // send
            expiredPolicyRenewal,               // do not send
            cancelledPolicyRenewal,             // do not send
            orderGeneratedPolicyRenewal,        // send
            policyPreviouslyRenewed,            // do not send
        };
    }
}
