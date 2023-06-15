using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Entity.Renewals;
using BizCover.Messages.Renewals;
using Moq;
using Xunit;

namespace BizCover.Application.Renewals.Tests.UseCases;

public class StartAutoRenewalOrderGenerationTests
{
    private readonly Mock<IQueuePublisher> _mockQueuePublisher = new();
    private readonly FakeRepository<Renewal> _fakeRepository = new();
    private readonly Mock<IRenewalConfigService> _mockAutoRenewalConfigService = new();

    private readonly StartAutoRenewalOrderGeneration _startAutoRenewalOrderGeneration;

    public StartAutoRenewalOrderGenerationTests()
    {
        _startAutoRenewalOrderGeneration = new StartAutoRenewalOrderGeneration(_mockQueuePublisher.Object, _fakeRepository);
    }

    [Fact]
    public async Task Run_Should_Only_Publish_Valid_PolicyIds_For_AutoRenewal_When_Executed()
    {
        var expiringPolicyId = Guid.NewGuid();
        _fakeRepository.Entities = GetRenewals(expiringPolicyId);

        _mockQueuePublisher.Setup(x => x.Send(It.IsAny<GenerateAutoRenewalOrderCommand>(), CancellationToken.None));
        _mockAutoRenewalConfigService.Setup(x => x.CanAutoRenew(It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns(true);

        await _startAutoRenewalOrderGeneration.Run(CancellationToken.None);

        _mockQueuePublisher.Verify(x => x.Send(It.IsAny<GenerateAutoRenewalOrderCommand>(), CancellationToken.None), Times.Exactly(1));
    }

    private static IEnumerable<Renewal> GetRenewals(Guid expiringPolicyId)
    {
        var expiringOptInAutoRenewalPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow, OrderGeneration = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active
        };

        var expiringOptOutAutoRenewalPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = Guid.NewGuid(),
            OrderId = null,
            OptIn = false,
            RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow, OrderGeneration = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active
        };

        var expiringPolicyNotInitiatedRenewal = new Renewal
        {
            ExpiringPolicyId = Guid.NewGuid(),
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiated = null, OrderGeneration = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active
        };

        var policyPreviouslyRenewed = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow, OrderGeneration = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            RenewedPolicyId = Guid.NewGuid()
        };

        //var optedOutExpiredPolicyRenewal = new Renewal
        //{
        //    ExpiringPolicyId = Guid.NewGuid(),
        //    OrderId = null,
        //    OptIn = false,
        //    RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow, OrderGeneration = DateTime.UtcNow.Date },
        //    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
        //    PolicyStatus = PolicyStatus.Active
        //};

        //var orderGeneratedExpiredPolicyRenewal = new Renewal
        //{
        //    ExpiringPolicyId = Guid.NewGuid(),
        //    OrderId = Guid.NewGuid(),
        //    OptIn = true,
        //    RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow, OrderGeneration = DateTime.UtcNow.Date },
        //    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
        //    PolicyStatus = PolicyStatus.Active
        //};

        //var ineligiblePolicyRenewal = new Renewal
        //{
        //    ExpiringPolicyId = expiringPolicyId,
        //    OrderId = null,
        //    OptIn = true,
        //    RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow, OrderGeneration = DateTime.UtcNow.Date },
        //    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = false },
        //    PolicyStatus = PolicyStatus.Active
        //};

        //var futureOrderGenerationPolicyRenewal = new Renewal
        //{
        //    ExpiringPolicyId = Guid.NewGuid(),
        //    OrderId = null,
        //    OptIn = true,
        //    RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow, OrderGeneration = DateTime.UtcNow.Date.AddYears(1) },
        //    AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
        //    PolicyStatus = PolicyStatus.Active
        //};

        return new List<Renewal>
        {
            expiringOptInAutoRenewalPolicyRenewal,
            expiringOptOutAutoRenewalPolicyRenewal,
            expiringPolicyNotInitiatedRenewal,
            policyPreviouslyRenewed
        };
    }
}
