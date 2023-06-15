using AutoFixture;
using AutoFixture.AutoNSubstitute;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Rules;
using BizCover.Application.Renewals.Rules.Criteria;
using BizCover.Entity.Renewals;
using FluentAssertions;
using Xunit;

namespace BizCover.Application.Renewals.Tests.Rules
{
    public class PolicyAlreadyRenewedCriteriaTests 
    {
        private readonly IFixture _fixture;
        private readonly PolicyAlreadyRenewedCriteria _sut;

        public PolicyAlreadyRenewedCriteriaTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _sut = _fixture.Create<PolicyAlreadyRenewedCriteria>();
        }
        

        [Fact]
        public void IsSatisfiedAsync_With_PolicyAlreadyRenewed_ReturnsFalse()
        {
            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .With(e=>e.RenewedPolicyId, Guid.NewGuid())
                .Create();


            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();


            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeFalse();
            result.Reason.Should().Be(string.Format(ValidationConstants.PolicyRenewed,
                request.Renewal.ExpiringPolicyId,
                request.Renewal.RenewedPolicyId.Value));
        }

        [Fact]
        public void IsSatisfiedAsync_With_PolicyNotRenewed_ReturnsTrue()
        {

            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .Without(e => e.RenewedPolicyId)
                .Create();


            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();

            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeTrue();
        }
    }
}
