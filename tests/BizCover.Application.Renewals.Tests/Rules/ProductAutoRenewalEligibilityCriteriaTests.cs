using AutoFixture;
using AutoFixture.AutoNSubstitute;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Rules;
using BizCover.Application.Renewals.Rules.Criteria;
using BizCover.Application.Renewals.Services;
using BizCover.Entity.Renewals;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BizCover.Application.Renewals.Tests.Rules
{
    public class ProductAutoRenewalEligibilityCriteriaTests
    {
        private readonly IFixture _fixture;
        private readonly IRenewalConfigService _renewalConfigService;
        private readonly ProductAutoRenewalEligibilityCriteria _sut;

        public ProductAutoRenewalEligibilityCriteriaTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _renewalConfigService = _fixture.Freeze<IRenewalConfigService>();
            _sut = _fixture.Create<ProductAutoRenewalEligibilityCriteria>();
        }


        [Fact]
        public void IsSatisfiedAsync_With_ProductAutoRenewalUnEligible_ReturnsFalse()
        {
            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .Create();
            
            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();

            _renewalConfigService.CanAutoRenew(request.Renewal.ProductCode, request.Renewal.PolicyInceptionDate)
                .Returns(false);

            var result = _sut.IsCriteriaSatisfiedAsync(request);

            result.IsEligible.Should().BeFalse();
            result.Reason.Should().Be(string.Format(ValidationConstants.ProductAutoUnEligible,
                request.Renewal.ProductCode));
        }

        [Fact]
        public void IsSatisfiedAsync_With_ProductAutoRenewalEligible_ReturnsTrue()
        {
            var getRenewalDetailsForExpiringPolicyResponse = _fixture.Build<Renewal>()
                .Create();


            var request = _fixture.Build<RenewalEligibilityCheckRequest>()
                .With(e => e.Renewal, getRenewalDetailsForExpiringPolicyResponse)
                .Create();

            _renewalConfigService.CanAutoRenew(request.Renewal.ProductCode, request.Renewal.PolicyInceptionDate)
                .Returns(true);

            var result = _sut.IsCriteriaSatisfiedAsync(request);


            result.IsEligible.Should().BeTrue();
        }
    }
}
