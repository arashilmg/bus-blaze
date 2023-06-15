using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BizCover.Application.Policies;
using BizCover.Application.Renewals.Services;
using Google.Protobuf.WellKnownTypes;

namespace BizCover.Api.Renewals.IntegrationTests.SubmitAutoRenewalOrder
{
    public class PolicyServiceStub : IPolicyService
    {
        public Task<PolicyDto> GetPolicy(string policyId)
        {
            return Task.FromResult(new PolicyDto()
            {
                PaymentFrequency = "Monthly",
                PolicyNumber = "ABC-001",
                ExpiryDate = DateTime.UtcNow.ToTimestamp()
            });
        }

        public Task<IEnumerable<PolicyDto>> FindPolicies(int offset, int fetch, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
