using BizCover.Application.Offers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Messages.Orders;
using MassTransit;

namespace BizCover.Consumer.Renewals
{
    public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
    {
        private readonly IOfferService _offerService;
        private readonly ISetPolicyAsRenewed _setPolicyAsRenewed;

        public OrderCompletedEventConsumer(IOfferService offerService, ISetPolicyAsRenewed setPolicyAsRenewed)
        {
            _offerService = offerService;
            _setPolicyAsRenewed = setPolicyAsRenewed;
        }

        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            foreach (var orderLineItem in context.Message.LineItems)
            {
                var offer = await _offerService.GetOffer(orderLineItem.OfferId.ToString());
                if (offer.OfferType != OfferType.Renewal)
                {
                    continue;
                }

                // We are assuming that in History last record always be OrderCompleted date
                var orderCompletedDate = context.Message.History.OrderByDescending(x => x.Item1).First().Item1;

                await _setPolicyAsRenewed.SetAsRenewed(
                     Guid.Parse(offer.ExpiringPolicyId),
                    orderLineItem.IntendedPolicyId,
                    // TODO:
                    // might be remove renewedDate from renewal context as renew is not responsible for those kind of detail 
                    // DDT team can get those details from order context when it was renewed
                    orderCompletedDate);  
            }
        }
    }
}
