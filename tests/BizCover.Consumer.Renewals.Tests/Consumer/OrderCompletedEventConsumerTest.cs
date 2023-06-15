using BizCover.Application.Offers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Messages.Orders;
using BizCover.Messages.Orders.Payloads;
using MassTransit;
using Moq;
using Xunit;

namespace BizCover.Consumer.Renewals.Tests.Consumer;

public class OrderCompletedEventConsumerTest
{
    private readonly Mock<IAddOrUpdatePolicyRenewalDetails> _addOrUpdatePolicyRenewalDetails = new();
    private readonly Mock<ISetPolicyAsRenewed> _setRenewedPolicyDetails = new();
    private readonly Mock<IOfferService> _offerService = new();

    private readonly OrderCompletedEventConsumer _orderCompletedEventConsumer;

    public OrderCompletedEventConsumerTest()
    {
        _orderCompletedEventConsumer = new OrderCompletedEventConsumer(_offerService.Object, _setRenewedPolicyDetails.Object);
    }

    [Theory]
    [InlineData(OfferType.New, false)]
    [InlineData(OfferType.Amendment, false)]
    [InlineData(OfferType.Renewal, true)]
    public async Task Can_Consume_OrderCompletedEvent(OfferType offerType, bool isExpectedToSetRenewedPolicyDetails)
    {
        var offerId = Guid.NewGuid();
        var policyId = Guid.NewGuid();
        var expiringPolicyId = Guid.NewGuid();

        var orderCompletedEvent = new TestOrderCompletedEvent()
        {
            LineItems = new List<LineItem>()
            {
                new()
                {
                    OfferId = offerId,
                    IntendedPolicyId = policyId
                }
            },
            History = new List<Tuple<DateTime, string>>()
            {
                Tuple.Create(DateTime.UtcNow, "Completed")
            }
        };

        var context = Mock.Of<ConsumeContext<OrderCompletedEvent>>(_ => _.Message == orderCompletedEvent);

        var offer = new OfferDto
        {
            OfferId = offerId.ToString(),
            OfferType = offerType,
            ExpiringPolicyId = expiringPolicyId.ToString()
        };

        _offerService.Setup(x => x.GetOffer(offerId.ToString())).Returns(Task.FromResult(offer));
        _setRenewedPolicyDetails.Setup(x => x.SetAsRenewed(expiringPolicyId, policyId, DateTime.Now));
        await _orderCompletedEventConsumer.Consume(context);
        _offerService.Verify(x => x.GetOffer(It.IsAny<string>()), Times.Once);
        var setRenewedPolicyDetailsTimes = isExpectedToSetRenewedPolicyDetails ? Times.Once() : Times.Never();
        _setRenewedPolicyDetails.Verify(x => x.SetAsRenewed(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()), setRenewedPolicyDetailsTimes);
    }


    class TestOrderCompletedEvent : OrderCompletedEvent
    {
        public Guid OrderId { get; } = default!;
        public IEnumerable<Guid> SoldOfferIds { get; } = default!;
        public Guid SoldCartId { get; } = default!;
        public IEnumerable<Tuple<DateTime, string>> History { get; set; } = default!;
        public Guid CartId { get; } = default!;
        public decimal TotalLineItemAmount { get; } = default!;
        public decimal TotalTransactionCharge { get; } = default!;
        public Guid ContactId { get; } = default!;
        public string ContactName { get; } = default!;
        public string ContactEmailAddress { get; } = default!;
        public string PaymentFrequency { get; } = default!;
        public string PaymentMethod { get; } = default!;
        public string CardType { get; } = default!;
        public IEnumerable<LegalEntity> LegalEntities { get; } = default!;
        public Messages.Orders.Payloads.LegalRepresentative LegalRepresentative { get; } = default!;
        public IEnumerable<LineItem> LineItems { get; set; } = default!;
        public IEnumerable<TransactionCharge> TransactionCharges { get; } = default!;
        public SubmittedBy SubmittedBy { get; } = default!;
    }
}
