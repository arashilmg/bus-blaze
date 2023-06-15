using AutoFixture;
using AutoFixture.AutoNSubstitute;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Rules;
using BizCover.Application.Renewals.Rules.Criteria;
using BizCover.Entity.Renewals;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace BizCover.Application.Renewals.Tests.Rules
{
    public class PolicyHasSpecialCircumstanceCriteriaTests
    {
        private readonly IFixture _fixture;
        private readonly PolicyHasSpecialCircumstanceCriteria _sut;
        private readonly IConfiguration _configuration;
        public PolicyHasSpecialCircumstanceCriteriaTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _configuration = _fixture.Freeze<IConfiguration>();
            _configuration[Arg.Is("RenewalMaxDays")].Returns("40");

            _sut = _fixture.Create<PolicyHasSpecialCircumstanceCriteria>();
        }


        [Fact]
        public void IsSatisfiedAsync_With_PolicyHasSpecialCircumstances_ReturnsFalse()
        {

            var specialCircumstancesDto = _fixture.Build<SpecialCircumstances>()
                .With(e => e.IsApplied, true)
                .Create();

            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .With(e => e.SpecialCircumstances, specialCircumstancesDto)
                .Create();

            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();

            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeFalse();
            result.Reason.Should().Be(string.Format(ValidationConstants.SpecialCircumstance,
                request.Renewal.ExpiringPolicyId));
        }

        [Fact]
        public void IsSatisfiedAsync_With_PolicyHasNoSpecialCircumstances_ReturnsTrue()
        {
            var specialCircumstancesDto = _fixture.Build<SpecialCircumstances>()
                .With(e => e.IsApplied, false)
                .Create();

            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .With(e => e.SpecialCircumstances, specialCircumstancesDto)
                .Create();

            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();

            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeTrue();
        }
    }
}
