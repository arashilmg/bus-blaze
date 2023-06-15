using BizCover.Api.Renewals.ComponentTests.Fixtures;
using BizCover.Application.Renewals;
using FluentAssertions;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests
{
    [Trait("Component", "Local")]
    public class IsEligibileForReQuoteRenewal : IClassFixture<ComponentTestFixture>
    {

        private readonly GrpcChannel _renewalsGrpcChannel;
        private readonly ComponentTestFixture _fixture;

        public IsEligibileForReQuoteRenewal(ComponentTestFixture fixture)
        {
            _fixture = fixture;
            _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
        }

        [Fact]
        public async Task Requote_renewal_is_eligible_When_policy_is_ineligible_for_autorenewal()
        {
            // arrange
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid().ToString();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate, "Active");

            // act
            await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);
            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
            // set policy level eligibility as false
            await client.UpdateAutoRenewalEligibilityAsync(new UpdateAutoRenewalEligibilityRequest() { Comments = "component test", ExpiringPolicyId = expiringPolicyId, IsEligible = false });

            // get renewal eligibility
            var response = await client.IsEligibleForReQuoteRenewalAsync(new IsEligibleForReQuoteRenewalRequest { ExpiringPolicyId = expiringPolicyId, });

            // assert
            response.IsEligible.Should().Be(true);
        }

        private object GetPolicyBoundEventForExpiringPolicy(string expiringPolicyId, DateTime policyBoundDate, string status)
        {
            var offerId = Guid.Parse(expiringPolicyId);
            return new
            {
                PolicyId = expiringPolicyId,
                PolicyNumber = "ABC-123",
                Status = status,
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
}
