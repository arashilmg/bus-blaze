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
    public class PolicyCancelledCriteriaTests 
    {
        private readonly IFixture _fixture;
        private readonly PolicyCancelledCriteria _sut;

        public PolicyCancelledCriteriaTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _sut = _fixture.Create<PolicyCancelledCriteria>();
        }


        [Fact]
        public void IsSatisfiedAsync_With_PolicyAlreadyCancelled_ReturnsFalse()
        {
            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .With(e => e.PolicyStatus, PolicyStatus.Cancelled)
                .Create();

            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();


            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeFalse();
            result.Reason.Should().Be(string.Format(ValidationConstants.PolicyIsNotActive,
                request.Renewal.ExpiringPolicyId,
                request.Renewal.PolicyStatus));
        }

        [Fact]
        public void IsSatisfiedAsync_With_PolicyNotCancelled_ReturnsTrue()
        {
            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .Without(e => e.PolicyStatus)
                .Create();

            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();

            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeTrue();
        }
    }
}
