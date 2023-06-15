using BizCover.Application.Policies;

namespace BizCover.Application.Renewals.Services;

internal class PolicyService : IPolicyService
{
    private readonly PoliciesService.PoliciesServiceClient _policiesClient;

    public PolicyService(PoliciesService.PoliciesServiceClient policiesClient) =>
        _policiesClient = policiesClient;

    public async Task<PolicyDto> GetPolicy(string policyId) =>
        (await _policiesClient.GetPolicyAsync(new GetPolicyRequest { PolicyId = policyId })).Policy;

    public async Task<IEnumerable<PolicyDto>> FindPolicies(int offset, int fetch, CancellationToken cancellationToken) =>
        (await _policiesClient.GetPoliciesByOffsetAsync(new GetPoliciesByOffsetRequest
            {
                Offset = offset,
                Fetch = fetch,
            }, cancellationToken: cancellationToken)
        ).Policies;
}
