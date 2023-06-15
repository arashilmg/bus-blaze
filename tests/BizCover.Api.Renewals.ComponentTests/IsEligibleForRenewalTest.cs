using AutoFixture.Xunit2;

using BizCover.Api.Renewals.ComponentTests.Fixtures;
using BizCover.Application.Renewals;
using BizCover.Messages.Orders.Payloads;
using FluentAssertions;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests
{
    [Trait("Component", "Local")]
    public class IsEligibleForRenewalTest : IClassFixture<ComponentTestFixture>
    {
        private readonly GrpcChannel _renewalsGrpcChannel;
        private readonly ComponentTestFixture _fixture;

        public IsEligibleForRenewalTest(ComponentTestFixture fixture)
        {
            _fixture = fixture;
            _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
        }

        [Theory,AutoData]
        public async Task When_Policy_is_eligible_it_should_return_true(Guid policyId)
        {
            // arrange
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = policyId.ToString();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate, "Active");

            // act
            await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);
            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
            var response = await client.IsEligibleForRenewalAsync(new IsEligibleForRenewalRequest() { ExpiringPolicyId = expiringPolicyId, });

            // assert
            response.IsEligible.Should().Be(true);
        }

        [Theory]
        [InlineData("Issued")]
        [InlineData("Cancelled")]
        public async Task When_Policy_Issued_or_Cancelled_Then_Renewal_Is_InEligible(string status)
        {
            // arrange
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid().ToString();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate, status);

            // act
            await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);
            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
            var response = await client.IsEligibleForRenewalAsync( new IsEligibleForRenewalRequest() { ExpiringPolicyId = expiringPolicyId, });

            // assert
            response.IsEligible.Should().Be(false);
        }

        [Fact]
        public async Task When_Policy_is_not_eligible_Then_Renewal_Is_InEligible()
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
            var response = await client.IsEligibleForRenewalAsync(new IsEligibleForRenewalRequest() { ExpiringPolicyId = expiringPolicyId, });

            // assert
            response.IsEligible.Should().Be(false);
        }

        [Fact]
        public async Task When_Special_circumstance_is_true_Then_Renewal_Is_InEligible()
        {
            // arrange
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = Guid.NewGuid().ToString();
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate, "Active");

            // act
            await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);
            
            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
            
            // set special circumstance to true
            await client.UpdateSpecialCircumstancesAsync(new UpdateSpecialCircumstancesRequest() { Comments = "component test", ExpiringPolicyId = expiringPolicyId, IsApplied = true, Reason = "claim" });

            // get renewal eligibility
            var response = await client.IsEligibleForRenewalAsync(new IsEligibleForRenewalRequest() { ExpiringPolicyId = expiringPolicyId, });

            // assert
            response.IsEligible.Should().Be(false);
        }

        [Fact]
        public async Task When_Policy_is_already_renewed_Then_Renewal_Is_InEligible()
        {
            // arrange & act
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = "00000000-0000-0000-0000-000000000001";
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate, "Active");

            // raise policy bound event for new policy
            await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);

            // raise order completed event for renewal policy
            var offerId = Guid.Parse("00000000-0000-0000-0000-000000000002");
            var renewedPolicyId = Guid.NewGuid();
            var orderCompletedEvent = new TestOrderCompletedEvent()
            {
                LineItems = new List<LineItem>()
                {
                    new()
                    {
                        OfferId = offerId,
                        IntendedPolicyId = renewedPolicyId
                    }
                },
                History = new List<Tuple<DateTime, string>>()
                {
                    Tuple.Create(DateTime.UtcNow, "Completed")
                }
            };
            await _fixture.PublishOrderCompletedEvent(orderCompletedEvent);

            // get renewal eligibility
            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
            var response = await client.IsEligibleForRenewalAsync(new IsEligibleForRenewalRequest() { ExpiringPolicyId = expiringPolicyId, });

            // assert
            response.IsEligible.Should().Be(false);
        }

        [Theory, AutoData]
        public async Task When_Policy_is_in_arrears_Then_Renewal_Is_InEligible(Guid policyId)
        {
            // arrange
            var policyBoundDate = new DateTime(2022, 3, 3);
            // this policyId has arrears in payment service fake
            var expiringPolicyId = policyId.ToString();
            expiringPolicyId = SetToPaymentStubValue(expiringPolicyId);
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate, "Active");

            // act
            await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);
            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);

            // get renewal eligibility
            var response = await client.IsEligibleForRenewalAsync(new IsEligibleForRenewalRequest() { ExpiringPolicyId = expiringPolicyId, });

            // assert
            response.IsEligible.Should().Be(false);
        }

        private static string SetToPaymentStubValue(string expiringPolicyId)
        {
            return expiringPolicyId.Replace(expiringPolicyId.Substring(expiringPolicyId.Length - 2), "11");
        }

        private object GetPolicyBoundEventForExpiringPolicy(string expiringPolicyId, DateTime policyBoundDate, string status, Guid? offerId = null)
        {
            if (offerId == null)
                offerId = Guid.Parse(expiringPolicyId);
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
