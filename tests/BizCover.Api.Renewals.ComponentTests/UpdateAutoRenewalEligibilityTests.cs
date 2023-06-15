using BizCover.Api.Renewals.ComponentTests.Fixtures;
using BizCover.Application.Renewals;
using FluentAssertions;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests;

[Trait("Component", "Local")]

public class UpdateAutoRenewalEligibilityTests : IClassFixture<ComponentTestFixture>
{
    private readonly GrpcChannel _renewalsGrpcChannel;
    private readonly ComponentTestFixture _fixture;

    public UpdateAutoRenewalEligibilityTests(ComponentTestFixture fixture)
    {
        _fixture = fixture;
        _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
    }

    [Theory]
    [InlineData(true, "good policy")]
    [InlineData(false, "bad policy")]
    public async Task Can_Set_AutoRenewal_Eligibility(bool isEligible, string comments)
    {
        var policyId = Guid.NewGuid();
        var policyBoundDate = DateTime.Now.AddDays(-300);

        var policyBoundEvent = new
        {
            PolicyId = policyId,
            PolicyNumber = "ABC-123",
            Status = "Active",
            OrderId = Guid.NewGuid().ToString(),
            QuotationId = Guid.NewGuid().ToString(),
            ContactId = Guid.NewGuid().ToString(),
            ProductCode = "PI-PL-DUAL",
            ProductTypeCode = "PI-PL",
            PolicyBoundDate = policyBoundDate,
            InceptionDate = new DateTime(2022, 3, 3),
            ExpiryDate = policyBoundDate.AddYears(1),
        };

        await _fixture.PublishPolicyBoundEvent(policyBoundEvent);

        // act
        var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
        await client.UpdateAutoRenewalEligibilityAsync(
            new UpdateAutoRenewalEligibilityRequest
            {
                ExpiringPolicyId = policyId.ToString(),
                IsEligible = isEligible,
                Comments = comments
            });

        // assert
        var actualRenewal = await client.GetRenewalDetailsForExpiringPolicyAsync(
            new GetRenewalDetailsForExpiringPolicyRequest { ExpiringPolicyId = policyId.ToString() });
        
        actualRenewal.Should().NotBeNull();
        actualRenewal.ExpiringPolicyId.Should().Be(policyId.ToString());
        actualRenewal.AutoRenewalEligibility.Should().NotBeNull();
        actualRenewal.AutoRenewalEligibility.IsEligible.Should().Be(isEligible);
        actualRenewal.AutoRenewalEligibility.Comments.Should().Be(comments);
    }
}
