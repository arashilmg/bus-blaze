using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Services;
using BizCover.Messages.Renewals;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BizCover.Consumer.Renewals
{
    public class SubmitAutoRenewalOrderCommandConsumer : IConsumer<SubmitAutoRenewalOrderCommand>
    {
        private readonly ILogger<SubmitAutoRenewalOrderCommandConsumer> _logger;
        private readonly IOrderService _orderService;
        private readonly IPolicyService _policyService;
        private readonly IRenewalEligibility _renewalEligibility;
        private readonly IPaymentService _paymentService;
        private readonly IQueuePublisher _queuePublisher;

        public SubmitAutoRenewalOrderCommandConsumer(ILogger<SubmitAutoRenewalOrderCommandConsumer> logger,
            IOrderService orderService, 
            IPolicyService policyService, 
            IRenewalEligibility renewalEligibility, 
            IPaymentService paymentService, 
            IQueuePublisher queuePublisher)
        {
            _logger = logger;
            _orderService = orderService;
            _policyService = policyService;
            _renewalEligibility = renewalEligibility;
            _paymentService = paymentService;
            _queuePublisher = queuePublisher;
        }

        public async Task Consume(ConsumeContext<SubmitAutoRenewalOrderCommand> context)
        {

            await _orderService.SetAutoRenewalUserDetails(context.Message.OrderId);

            var expiringPolicy = await _policyService.GetPolicy(context.Message.ExpiringPolicyId.ToString());
            
            var result = await _renewalEligibility.CheckEligibility(context.Message.ExpiringPolicyId, expiringPolicy.PaymentFrequency, context.CancellationToken);

            if (!result.IsEligible)
            {
                _logger.LogInformation("Unable to submit auto renewal order because {reason}", result.Reason);
                return;
            }

            var subscription = await _paymentService.GetSubscriptionByPolicyId(context.Message.ExpiringPolicyId);

            await _paymentService.CreatePendingPayment(context.Message.OrderId, subscription.CreditCard.PaymentProfileId);

            await _queuePublisher.Publish(new AutoRenewalPendingPaymentCreatedEvent()
            {
                ExpiringPolicyId = context.Message.ExpiringPolicyId,
                OrderId = context.Message.OrderId
            }, context.CancellationToken);
        }

    }
}
