using System;
using System.Threading;
using System.Threading.Tasks;
using BizCover.Application.Renewals.Rules.AutoEligibility;

namespace BizCover.Api.Renewals.IntegrationTests.SubmitAutoRenewalOrder
{
    public class RenewalEligibilityStub : IRenewalEligibility
    {
        public Task<RenewalEligibilityResponse> CheckEligibility(Guid expiringPolicyId, string paymentFrequency, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RenewalEligibilityResponse()
            {
                IsEligible = true
            });
        }
    }
}
