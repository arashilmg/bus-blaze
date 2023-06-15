using BizCover.gRPC.Payment;
using Google.Type;
using Grpc.Core;

namespace BizCover.Api.PaymentFake.Services
{
    public class GrpcPaymentService : PaymentService.PaymentServiceBase
    {
        public override Task<GetArrearsBreakdownByPolicyIdResponse> GetArrearsBreakdownByPolicyId(GetArrearsBreakdownByPolicyIdRequest request, ServerCallContext context)
        {
            if (request.PolicyId.EndsWith("11"))
            {
                return Task.FromResult(new GetArrearsBreakdownByPolicyIdResponse()
                {
                    TotalArrearsAmount = new Money()
                    {
                        DecimalValue = 100
                    }
                });
            }

            return Task.FromResult(new GetArrearsBreakdownByPolicyIdResponse()
            {
                TotalArrearsAmount = new Money()
                {
                    DecimalValue = 0
                }
            });
        }
    }
}
