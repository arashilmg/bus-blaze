using BizCover.Application.Renewals.Services;
using BizCover.Messages.Renewals;
using MassTransit;

namespace BizCover.Consumer.Renewals
{
    public class AutoRenewalPendingPaymentCreatedEventConsumer : IConsumer<AutoRenewalPendingPaymentCreatedEvent>
    {
        private readonly IOrderService _orderService;
        private readonly IRenewalService _renewalService;

        public AutoRenewalPendingPaymentCreatedEventConsumer(IOrderService orderService, IRenewalService renewalService)
        {
            _orderService = orderService;
            _renewalService = renewalService;
        }

        public async Task Consume(ConsumeContext<AutoRenewalPendingPaymentCreatedEvent> context)
        {
            await _orderService.SubmitOrder(context.Message.OrderId);

            await _renewalService.SetOrderSubmissionDetails(context.Message.ExpiringPolicyId, context.CancellationToken);
        }
    }
}
