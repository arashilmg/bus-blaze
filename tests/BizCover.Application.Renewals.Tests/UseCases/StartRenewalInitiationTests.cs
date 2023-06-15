using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.UseCases;
using BizCover.Entity.Renewals;
using BizCover.Messages.Renewals;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BizCover.Application.Renewals.Tests.UseCases;

public class StartRenewalInitiationTests
{
    private readonly Mock<IQueuePublisher> _mockQueuePublisher = new();
    private readonly FakeRepository<Renewal> _fakeRepository = new();
    private readonly Mock<ILogger<StartRenewalInitiation>> _mockLogger = new();

    private readonly StartRenewalInitiation _startRenewalInitiation;

    public StartRenewalInitiationTests()
    {
        _startRenewalInitiation = new StartRenewalInitiation(_mockQueuePublisher.Object, _fakeRepository, _mockLogger.Object);
    }

    [Fact]
    public async Task Run_Should_Only_Publish_Valid_PolicyIds_When_Executed()
    {
        var expiringPolicyId = Guid.NewGuid();
        _fakeRepository.Entities = GetRenewals(expiringPolicyId);

        _mockQueuePublisher.Setup(x => x.Send(It.IsAny<InitiateRenewalCommand>(), CancellationToken.None));
       
        await _startRenewalInitiation.Run(CancellationToken.None);

        _mockQueuePublisher.Verify(x => x.Send(It.IsAny<InitiateRenewalCommand>(), CancellationToken.None),
            Times.Exactly(7));
        
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
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled {  IsEnabled = true },
            PolicyStatus = PolicyStatus.Active
        };

        var cancelledPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Cancelled
        };

        var expiredPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = Guid.NewGuid(),
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Expired
        };
        
        var outInFalsePolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = false,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Active
        };

        var orderInitiatedExpiringPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiated = DateTime.UtcNow.Date, Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Active
        };

        var ineligibleExpiringPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = false },
            PolicyStatus = PolicyStatus.Active
        };

        var futureInitiationExpiringPolicyRenewal = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date.AddYears(1) },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Active
        };

        var policyWithNullSpecialCircumstances = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            PolicyStatus = PolicyStatus.Active,
            SpecialCircumstances = null
        };

        var policyWithFalseSpecialCircumstances = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = true },
            SpecialCircumstances = new SpecialCircumstances()
            {
                IsApplied = false
            }
        };

        var policyWithTrueSpecialCircumstances = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            SpecialCircumstances = new SpecialCircumstances()
            {
                IsApplied = true
            }
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

        var policyOptedOutRenewed = new Renewal
        {
            ExpiringPolicyId = expiringPolicyId,
            OrderId = null,
            OptIn = true,
            RenewalDates = new RenewalDates { Initiation = DateTime.UtcNow.Date },
            AutoRenewalEligibility = new AutoRenewalEligibility { IsEligible = true },
            PolicyStatus = PolicyStatus.Active,
            AllRenewalsEnabled = new RenewalsEnabled { IsEnabled = false }
        };

        return new List<Renewal>
        {
            activePolicyRenewal,                        // Initiate renewal
            outInFalsePolicyRenewal,                    // Initiate renewal    
            ineligibleExpiringPolicyRenewal,            // Initiate renewal
            policyWithNullSpecialCircumstances,         // Initiate renewal
            policyWithFalseSpecialCircumstances,        // Initiate renewal
            policyOptedOutRenewed,                      // Initiate renewal
            policyWithTrueSpecialCircumstances,         // Initiate renewal
            issuedPolicyRenewal,                        // Do not initiate renewal
            expiredPolicyRenewal,                       // Do not initiate renewal
            cancelledPolicyRenewal,                     // Do not initiate renewal
            futureInitiationExpiringPolicyRenewal,      // Do not initiate renewal
            orderInitiatedExpiringPolicyRenewal,        // Do not initiate renewal
            policyPreviouslyRenewed,                    // Do not initiate renewal
        };
    }
}