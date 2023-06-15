using BizCover.Api.Renewals.ComponentTests.Fixtures;
using BizCover.Application.Renewals;
using FluentAssertions;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests
{
    [Trait("Component", "Local")]
    public class UpdateAutoRenewalOptInFlagTests : IClassFixture<ComponentTestFixture>
    {
        private readonly ComponentTestFixture _fixture;
        private readonly GrpcChannel _renewalsGrpcChannel;

        public UpdateAutoRenewalOptInFlagTests(ComponentTestFixture fixture)
        {
            _fixture = fixture;
            _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateAutoRenewalOptInFlag_Should_Update_OptIn_Properly_When_The_EndPoint_Is_called(bool isOptIn)
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

            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
            await client.UpdateAutoRenewalOptInFlagAsync(
                new UpdateAutoRenewalOptInFlagRequest
                {
                    ExpiringPolicyId = policyId.ToString(),
                    OptIn = isOptIn
                });


            var actualRenewal = await client.GetRenewalDetailsForExpiringPolicyAsync(
                new GetRenewalDetailsForExpiringPolicyRequest { ExpiringPolicyId = policyId.ToString() });

            actualRenewal.Should().NotBeNull();
            actualRenewal.OptIn.Should().Be(isOptIn);
        }
    }
}
