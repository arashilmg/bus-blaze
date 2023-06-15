using BizCover.Api.Renewals.ComponentTests.Fixtures;
using BizCover.Application.Renewals;
using FluentAssertions;

using Grpc.Core;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests
{
    [Trait("Component", "Local")]
    public class UpdateSpecialCircumstancesTests : IClassFixture<ComponentTestFixture>
    {
        private readonly ComponentTestFixture _fixture;
        private readonly GrpcChannel _renewalsGrpcChannel;

        public UpdateSpecialCircumstancesTests(ComponentTestFixture fixture)
        {
            _fixture = fixture;
            _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
        }

        [Theory]
        [InlineData(true, "Comment TRUE", "It Bug")]
        [InlineData(false, "Comment FALSE", "It Bug")]
        public async Task UpdateSpecialCircumstances_Should_Update_Properly_When_The_EndPoint_Is_called(bool hasSpecialCircumstancesApplied, string comment, string reason)
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
            await client.UpdateSpecialCircumstancesAsync(
                new UpdateSpecialCircumstancesRequest
                {
                    ExpiringPolicyId = policyId.ToString(),
                    IsApplied = hasSpecialCircumstancesApplied,
                    Comments = comment,
                    Reason = reason
                });


            var actualRenewal = await client.GetRenewalDetailsForExpiringPolicyAsync(
                new GetRenewalDetailsForExpiringPolicyRequest { ExpiringPolicyId = policyId.ToString() });

            actualRenewal.SpecialCircumstances.Should().NotBeNull();
            actualRenewal.SpecialCircumstances.IsApplied.Should().Be(hasSpecialCircumstancesApplied);
            actualRenewal.SpecialCircumstances.Comments.Should().BeEquivalentTo(comment);
            actualRenewal.SpecialCircumstances.UpdatedAt.Should().NotBeNull();

            if (hasSpecialCircumstancesApplied)
                actualRenewal.SpecialCircumstances.Reason.Should().BeEquivalentTo(reason);

        }

        [Theory]
        [InlineData("Comment TRUE", "insurer request")]
        public async Task UpdateSpecialCircumstances_InsurerBlock_ValidationShould_Fail(string comment, string reason)
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
            await Assert.ThrowsAsync<RpcException>(async () =>
               await client.UpdateSpecialCircumstancesAsync(
                   new UpdateSpecialCircumstancesRequest
                   {
                       ExpiringPolicyId = policyId.ToString(),
                       IsApplied = true,
                       Comments = comment,
                       Reason = reason
                   }));
        }


        [Theory]
        [InlineData("Comment TRUE", "random" , "insurer block")]
        public async Task UpdateSpecialCircumstances_ValidationShould_Fail(string comment, string reason, string reason2)
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
            await Assert.ThrowsAsync<RpcException>( async () => 
                await client.UpdateSpecialCircumstancesAsync(
                    new UpdateSpecialCircumstancesRequest
                    {
                        ExpiringPolicyId = policyId.ToString(),
                        IsApplied = true,
                        Comments = comment,
                        Reason = reason,
                        SecondLevelReason = reason2
                    }));
        }
    }
}
