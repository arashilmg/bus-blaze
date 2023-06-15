using BizCover.gRPC.Payment;

namespace BizCover.Application.Renewals.Services
{
    public interface IPaymentService
    {
        Task<bool> HasArrears(Guid expiringPolicyId);
        Task<SubscriptionDto> GetSubscriptionByPolicyId(Guid expiringPolicyId);
        Task CreatePendingPayment(Guid orderId, string chargifyPaymentProfileId);
    }
}
