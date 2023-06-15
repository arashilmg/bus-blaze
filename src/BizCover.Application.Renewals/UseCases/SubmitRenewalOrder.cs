using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Services;

namespace BizCover.Application.Renewals.UseCases
{
    public class SubmitRenewalOrder : ISubmitRenewalOrder
    {
        private readonly IOrderService _orderService;
        private readonly IPolicyService _policyService;
        private readonly IRenewalEligibility _renewalEligibility;
        private readonly IPaymentService _paymentService;

        public SubmitRenewalOrder(IOrderService orderService, IPolicyService policyService, IRenewalEligibility renewalEligibility, IPaymentService paymentService)
        {
            _orderService = orderService;
            _policyService = policyService;
            _renewalEligibility = renewalEligibility;
            _paymentService = paymentService;
        }

        public async Task<SubmitOrderResponse> Submit(Guid expiringPolicyId, Guid orderId, CancellationToken cancellationToken)
        {
            var expiringPolicy = await _policyService.GetPolicy(expiringPolicyId.ToString());
            var result = await _renewalEligibility.CheckEligibility(expiringPolicyId, expiringPolicy.PaymentFrequency, cancellationToken);

            if (!result.IsEligible)
            {
                return new SubmitOrderResponse()
                {
                    Success = false,
                    Result = new ResultDto()
                    {
                        FailReason = result.Reason
                    }
                };
            }

            var subscription = await _paymentService.GetSubscriptionByPolicyId(expiringPolicyId);

            await _paymentService.CreatePendingPayment(orderId, subscription.CreditCard.PaymentProfileId);

            await _orderService.SubmitOrder(orderId);

            return new SubmitOrderResponse()
            {
                Success = true,
                Result = new ResultDto()
                {
                    OrderId = orderId
                }
            };
        }
    }

    public interface ISubmitRenewalOrder
    {
        Task<SubmitOrderResponse> Submit(Guid expiringPolicyId, Guid orderId, CancellationToken cancellationToken);
    }

    public class SubmitOrderResponse
    {
        public bool Success { get; set; }
        public ResultDto Result { get; set; }
    }

    public class ResultDto
    {
        public string FailReason { get; set; }
        public Guid OrderId { get; set; }
    }
}
