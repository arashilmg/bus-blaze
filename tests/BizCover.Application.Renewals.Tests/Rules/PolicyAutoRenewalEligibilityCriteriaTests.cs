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
    public class PolicyAutoRenewalEligibilityCriteriaTests
    {
        private readonly IFixture _fixture;
        private readonly PolicyAutoRenewalEligibilityCriteria _sut;

        public PolicyAutoRenewalEligibilityCriteriaTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _sut = _fixture.Create<PolicyAutoRenewalEligibilityCriteria>();
        }


        [Fact]
        public void IsSatisfiedAsync_With_PolicyAutoRenewalUnEligible_ReturnsFalse()
        {
            var autoRenewalEligibility = _fixture.Build<AutoRenewalEligibility>()
                .With(e => e.IsEligible, false)
                .Create();

            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .With(e => e.AutoRenewalEligibility, autoRenewalEligibility)
                .Create();


            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();


            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeFalse();
            result.Reason.Should().Be(string.Format(ValidationConstants.PolicyAutoUnEligible,
                request.Renewal.ExpiringPolicyId,
                request.Renewal.AutoRenewalEligibility!.Comments));
        }

        [Fact]
        public void IsSatisfiedAsync_With_PolicyAutoRenewalEligible_ReturnsTrue()
        {

            var autoRenewalEligibility = _fixture.Build<AutoRenewalEligibility>()
                .With(e => e.IsEligible, true)
                .Create();

            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .With(e => e.AutoRenewalEligibility, autoRenewalEligibility)
                .Create();


            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();


            var result = _sut.IsCriteriaSatisfiedAsync(request);

            
            result.IsEligible.Should().BeTrue();
        }
    }
}
