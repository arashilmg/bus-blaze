using BizCover.Api.Renewals.ComponentTests.Fixtures;
using BizCover.Application.Renewals;
using BizCover.Messages.Renewals;
using FluentAssertions;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests;

[Trait("Component", "Local")]

public class PolicyBoundEventTests : IClassFixture<ComponentTestFixture>
{
    private readonly GrpcChannel _renewalsGrpcChannel;
    private readonly ComponentTestFixture _fixture;

    public PolicyBoundEventTests(ComponentTestFixture fixture)
    {
        _fixture = fixture;
        _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
    }


    [Fact]
    public async Task Can_Handle_PolicyBoundEvent_for_New_Policy()
    {
        // arrange
        var policyBoundDate = new DateTime(2022, 3, 3);
        var expiringPolicyId = "00000000-0000-0000-0000-000000000003";
        var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate);

        // act
        await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);

        // assert
        var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
        var actualRenewal = await client.GetRenewalDetailsForExpiringPolicyAsync(
            new GetRenewalDetailsForExpiringPolicyRequest { ExpiringPolicyId = expiringPolicyId });

        actualRenewal.Should().NotBeNull();
        actualRenewal.ExpiringPolicyId.Should().Be(expiringPolicyId);
        actualRenewal.RenewalDates.Should().NotBeNull();
        actualRenewal.RenewalDates.Initiation.ToDateTime().ToUniversalTime().Should().Be(policyBoundDate.AddYears(1).AddDays(-40).ToUniversalTime());
        actualRenewal.RenewalDates.OrderGeneration.ToDateTime().ToUniversalTime().Should().Be(policyBoundDate.AddYears(1).AddDays(-21).ToUniversalTime());
        actualRenewal.RenewalDates.OrderSubmission.ToDateTime().ToUniversalTime().Should().Be(policyBoundDate.AddYears(1).AddDays(-7).ToUniversalTime());
        actualRenewal.PolicyExpiryDate.Should().NotBeNull();
        actualRenewal.PolicyInceptionDate.Should().NotBeNull();
        actualRenewal.OptIn.Should().BeTrue();

        actualRenewal.AutoRenewalEligibility.Should().NotBeNull();
        actualRenewal.AutoRenewalEligibility.IsEligible.Should().BeTrue();
        actualRenewal.AutoRenewalEligibility.UpdatedAt.Should().NotBeNull();

        actualRenewal.SpecialCircumstances.Should().NotBeNull();
        actualRenewal.SpecialCircumstances.IsApplied.Should().BeFalse();
        actualRenewal.SpecialCircumstances.UpdatedAt.Should().NotBeNull();

        actualRenewal.RenewedPolicyId.Should().BeEmpty();
        actualRenewal.RenewalType.Should().BeEmpty();
    }

    [Fact]
    public async Task Can_Handle_MigratePolicyCommand()
    {
        var policyId = Guid.NewGuid();
        var policyBoundDate = DateTime.Now.AddDays(-300);

        var migratePolicyCommand = new MigratePolicyCommand
        {
            PolicyId = policyId.ToString(),
            ProductCode = "PI-PL-DUAL",
            ExpiryDate = policyBoundDate.AddYears(1),
            InceptionDate = policyBoundDate.AddYears(1),
            Status = "Active"
        };

        // act
        await _fixture.PublishMigratePolicyCommand(migratePolicyCommand);

        // assert
        var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
        var actualRenewal = await client.GetRenewalDetailsForExpiringPolicyAsync(
            new GetRenewalDetailsForExpiringPolicyRequest { ExpiringPolicyId = policyId.ToString() });

        actualRenewal.Should().NotBeNull();
        actualRenewal.ExpiringPolicyId.Should().Be(policyId.ToString());
        actualRenewal.RenewalDates.Should().NotBeNull();
        actualRenewal.RenewalDates.Initiation.ToDateTime().Date.Should().Be(policyBoundDate.AddYears(1).AddDays(-40).Date);
        actualRenewal.RenewalDates.OrderGeneration.ToDateTime().Date.Should().Be(policyBoundDate.AddYears(1).AddDays(-21).Date);
        actualRenewal.RenewalDates.OrderSubmission.ToDateTime().Date.Should().Be(policyBoundDate.AddYears(1).AddDays(-7).Date);
        actualRenewal.PolicyExpiryDate.Should().NotBeNull();
        actualRenewal.PolicyInceptionDate.Should().NotBeNull();
        actualRenewal.OptIn.Should().BeTrue();

        actualRenewal.AutoRenewalEligibility.Should().NotBeNull();
        actualRenewal.AutoRenewalEligibility.IsEligible.Should().BeTrue();
        actualRenewal.AutoRenewalEligibility.UpdatedAt.Should().NotBeNull();

        actualRenewal.SpecialCircumstances.Should().NotBeNull();
        actualRenewal.SpecialCircumstances.IsApplied.Should().BeFalse();
        actualRenewal.SpecialCircumstances.UpdatedAt.Should().NotBeNull();
    }


    private object GetPolicyBoundEventForExpiringPolicy(string expiringPolicyId, DateTime policyBoundDate)
    {
        var offerId = Guid.Parse(expiringPolicyId);
        return new
        {
            PolicyId = expiringPolicyId,
            PolicyNumber = "ABC-123",
            Status = "Active",
            OrderId = Guid.NewGuid().ToString(),
            QuotationId = Guid.NewGuid().ToString(),
            ContactId = Guid.NewGuid().ToString(),
            ProductCode = "PI-PL-DUAL",
            ProductTypeCode = "PI-PL",
            PolicyBoundDate = policyBoundDate,
            InceptionDate = policyBoundDate,
            ExpiryDate = policyBoundDate.AddYears(1),
            OfferId = offerId
        };
    }
}
