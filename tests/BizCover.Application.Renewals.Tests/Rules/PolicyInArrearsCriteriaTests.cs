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
    public class PolicyInArrearsCriteriaTests
    {
        private readonly IFixture _fixture;
        private readonly PolicyInArrearsCriteria _sut;

        public PolicyInArrearsCriteriaTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _sut = _fixture.Create<PolicyInArrearsCriteria>();
        }


        [Fact]
        public void IsSatisfiedAsync_With_PolicyHasArrears_ReturnsFalse()
        {
            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Create<Renewal>();
            
            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .With(e => e.HasArrears, true)
                .Create();
            
            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeFalse();
            result.Reason.Should().Be(string.Format(ValidationConstants.PolicyInArrears,
                request.Renewal.ExpiringPolicyId));
        }

        [Fact]
        public void IsSatisfiedAsync_With_PolicyHasArrears_ReturnsTrue()
        {
            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Create<Renewal>();

            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .With(e=>e.HasArrears, false)
                .Create();
            
            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeTrue();
        }

        
    }
}
