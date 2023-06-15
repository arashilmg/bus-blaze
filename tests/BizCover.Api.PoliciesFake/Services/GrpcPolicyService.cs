using BizCover.Application.Policies;
using Grpc.Core;

namespace BizCover.Api.PolicyFake.Services;

public class GrpcPolicyService : PoliciesService.PoliciesServiceBase
{
    public override Task<GetPolicyResponse> GetPolicy(GetPolicyRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetPolicyResponse()
        {
            Policy = new PolicyDto()
            {
                PaymentFrequency = "Monthly"
            }
        });
    }
}