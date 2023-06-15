using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Entity.Renewals;
using BizCover.Messages.Renewals;
using Moq;
using Xunit;

namespace BizCover.Application.Renewals.Tests.UseCases;

public class StartRenewalOrderSubmissionTests
{
    private readonly Mock<IQueuePublisher> _mockQueuePublisher = new();
    private readonly FakeRepository<Renewal> _fakeRepository = new();
    private readonly StartAutoRenewalOrderSubmission _startRenewalOrderGeneration;

    public StartRenewalOrderSubmissionTests()
    {
        _startRenewalOrderGeneration = new StartAutoRenewalOrderSubmission(_mockQueuePublisher.Object, _fakeRepository);
    }

    [Fact]
    public async Task Run_Should_Only_Publish_Valid_PolicyIds_When_Executed()
    {
        var expiringPolicyId = Guid.NewGuid();
        _fakeRepository.Entities = GetRenewals(expiringPolicyId);

        _mockQueuePublisher.Setup(x => x.Send(It.IsAny<SubmitAutoRenewalOrderCommand>(), CancellationToken.None));

        await _startRenewalOrderGeneration.Run(CancellationToken.None);

        _mockQueuePublisher.Verify(x => x.Send(It.IsAny<SubmitAutoRenewalOrderCommand>(), CancellationToken.None),
            Times.Once); // only one event should be raised
        _mockQueuePublisher.Verify(
            x => x.Send(
                It.Is<SubmitAutoRenewalOrderCommand>(p => p.ExpiringPolicyId == expiringPolicyId),
                CancellationToken.None), Times.Once); // just making sure the correct policy is used.
    }

    [Fact]
    public async Task Run_Should_Only_Publish_Valid_PolicyIds_Including_Retry_When_Executed()
    {
        var expiringPolicyId = Guid.NewGuid();
        _fakeRepository.Entities = GetRenewals(expiringPolicyId, includeFailed:true);

        _mockQueuePublisher.Setup(x => x.Send(It.IsAny<SubmitAutoRenewalOrderCommand>(), CancellationToken.None));

        await _startRenewalOrderGeneration.Run(CancellationToken.None);

        _mockQueuePublisher.Verify(x => x.Send(It.IsAny<SubmitAutoRenewalOrderCommand>(), CancellationToken.None),
            Times.Exactly(3));

        _mockQueuePublisher.Verify(
            x => x.Send(
                It.Is<SubmitAutoRenewalOrderCommand>(p => p.ExpiringPolicyId == expiringPolicyId),
                CancellationToken.None), Times.Exactly(3));
    }

    private static IEnumerable<Renewal> GetRenewals(Guid expiringPolicyId, bool includeFailed = false)
    {
        var expiringPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates { OrderGenerated = DateTime.UtcNow, OrderSubmission = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active
        };

        var expiringPolicyRenewalInNextDay = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates { OrderGenerated = DateTime.UtcNow, OrderSubmission = DateTime.UtcNow.Date.AddDays(1) },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
        };

        var expiredPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates { OrderGenerated = DateTime.UtcNow, OrderSubmission = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Expired
        };

        var notGeneratedPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates { OrderSubmission = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active
        };

        var optedOutExpiredPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = false,
            RenewalDates = new RenewalDates { OrderGenerated = DateTime.UtcNow, OrderSubmission = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active
        };

        var orderSubmittedExpiredPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates
            {
                OrderGenerated = DateTime.UtcNow, OrderSubmission = DateTime.UtcNow.Date, OrderSubmitted = DateTime.UtcNow.Date
            },
            AutoRenewalEligibility = new AutoRenewalEligibility {IsEligible = true},
            PolicyStatus = PolicyStatus.Active,
            RenewedPolicyId = Guid.NewGuid()
        };

        var futureOrderSubmissionPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates { OrderGenerated = DateTime.UtcNow, OrderSubmission = DateTime.UtcNow.Date.AddYears(1) },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active
        };

        var orderSubmittedFailedPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates
            {
                OrderGenerated = DateTime.UtcNow,
                OrderSubmission = DateTime.UtcNow.AddDays(-1).Date,
                OrderSubmitted = DateTime.UtcNow.AddDays(-1).Date
            },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            RenewedPolicyId = null
        };

        var orderSubmittedFailedPolicyRenewalPastCutOff = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates
            {
                OrderGenerated = DateTime.UtcNow,
                OrderSubmission = DateTime.UtcNow.AddDays(-7).Date,
                OrderSubmitted = DateTime.UtcNow.AddDays(-7).Date
            },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            RenewedPolicyId = null
        };

        var orderSubmittedFailedPolicyRenewalWithNullOrderSubmitted = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates
            {
                OrderGenerated = DateTime.UtcNow,
                OrderSubmission = DateTime.UtcNow.AddDays(-1).Date,
                OrderSubmitted = null
            },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            RenewedPolicyId = null
        };

        var policyPreviouslyRenewed = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = Guid.NewGuid(),
            OptIn = true,
            RenewalDates = new RenewalDates { OrderGenerated = DateTime.UtcNow, OrderSubmission = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            RenewedPolicyId = Guid.NewGuid()
        };

        var renewals = new List<Renewal>
        {
            expiringPolicyRenewal,
            expiringPolicyRenewalInNextDay,
            expiredPolicyRenewal,
            notGeneratedPolicyRenewal,
            optedOutExpiredPolicyRenewal,
            orderSubmittedExpiredPolicyRenewal,
            futureOrderSubmissionPolicyRenewal,
            policyPreviouslyRenewed
        };

        if (includeFailed)
        {
            renewals.Add(orderSubmittedFailedPolicyRenewal);
            renewals.Add(orderSubmittedFailedPolicyRenewalPastCutOff);
            renewals.Add(orderSubmittedFailedPolicyRenewalWithNullOrderSubmitted);
        }

        return renewals;
    }
}
