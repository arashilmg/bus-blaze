using System;
using System.Threading.Tasks;
using BizCover.Application.Renewals.Services;
using BizCover.gRPC.Payment;

namespace BizCover.Api.Renewals.IntegrationTests.SubmitAutoRenewalOrder
{
    public class PaymentServiceStub : IPaymentService
    {
        public Task<bool> HasArrears(Guid expiringPolicyId)
        {
            return Task.FromResult(true);
        }

        public Task<SubscriptionDto> GetSubscriptionByPolicyId(Guid expiringPolicyId)
        {
            return Task.FromResult(new SubscriptionDto()
            {
                CreditCard = new CreditCardDto()
                {
                    PaymentProfileId = "321"
                }
            });
        }

        public Task CreatePendingPayment(Guid orderId, string chargifyPaymentProfileId)
        {
            return Task.CompletedTask;
        }
    }
}
