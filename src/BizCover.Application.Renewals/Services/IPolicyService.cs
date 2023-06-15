using BizCover.Application.Policies;

namespace BizCover.Application.Renewals.Services;

public interface IPolicyService
{
    Task<PolicyDto> GetPolicy(string policyId);
    Task<IEnumerable<PolicyDto>> FindPolicies(int offset, int fetch, CancellationToken cancellationToken);
}
