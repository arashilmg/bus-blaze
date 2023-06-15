using BizCover.Application.Orders;
using BizCover.gRPC.Payment;

namespace BizCover.Application.Renewals.Services
{
    internal class PaymentService : IPaymentService
    {
        private readonly gRPC.Payment.PaymentService.PaymentServiceClient _client;

        public PaymentService(gRPC.Payment.PaymentService.PaymentServiceClient client)
        {
            _client = client;
        }

        public async Task<bool> HasArrears(Guid expiringPolicyId)
        {
            var totalArrearsAmount = (await _client.GetArrearsBreakdownByPolicyIdAsync(new GetArrearsBreakdownByPolicyIdRequest()
            {
                PolicyId = expiringPolicyId.ToString()
            })).TotalArrearsAmount;

            return totalArrearsAmount.DecimalValue > 0;
        }

        public async Task<SubscriptionDto> GetSubscriptionByPolicyId(Guid expiringPolicyId)
        {
            return (await _client.GetSubscriptionByPolicyIdAsync(new GetSubscriptionByPolicyIdRequest()
            {
                PolicyId = expiringPolicyId.ToString()
            })).Subscription;
        }

        public async Task CreatePendingPayment(Guid orderId, string chargifyPaymentProfileId)
        {
            _ = (await _client.CreatePendingPaymentAsync(new CreatePendingPaymentRequest()
            {
                OrderId = orderId.ToString(),
                ChargifyPaymentProfileId = chargifyPaymentProfileId,
                PaymentId = Guid.NewGuid().ToString()
            }));
        }
    }
}
