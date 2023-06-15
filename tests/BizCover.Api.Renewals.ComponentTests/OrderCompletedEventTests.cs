using BizCover.Api.Renewals.ComponentTests.Fixtures;
using BizCover.Application.Renewals;
using BizCover.Messages.Orders;
using BizCover.Messages.Orders.Payloads;
using FluentAssertions;
using Grpc.Net.Client;
using Xunit;

namespace BizCover.Api.Renewals.ComponentTests
{
    [Trait("Component", "Local")]
    public class OrderCompletedEventTests : IClassFixture<ComponentTestFixture>
    {
        private readonly GrpcChannel _renewalsGrpcChannel;
        private readonly ComponentTestFixture _fixture;

        public OrderCompletedEventTests(ComponentTestFixture fixture)
        {
            _fixture = fixture;
            _renewalsGrpcChannel = GrpcChannel.ForAddress(new Uri(fixture.SvcGrpcUrl));
        }

        [Fact]
        public async Task Can_Handle_OrderCompleted_for_Renewal_Offer()
        {
            // arrange

            // setup expiring policy bound event
            var policyBoundDate = new DateTime(2022, 3, 3);
            var expiringPolicyId = "00000000-0000-0000-0000-000000000004";
            var expiringPolicyBoundEvent = GetPolicyBoundEventForExpiringPolicy(expiringPolicyId, policyBoundDate);
            await _fixture.PublishPolicyBoundEvent(expiringPolicyBoundEvent);

            // raise renewal order completed event
            var offerId = Guid.Parse("00000000-0000-0000-0000-000000000005");
            var renewedPolicyId = Guid.NewGuid();
            var orderCompletedEvent = new TestOrderCompletedEvent()
            {
                LineItems = new List<LineItem>()
                {
                    new()
                    {
                        OfferId = offerId,
                        IntendedPolicyId = renewedPolicyId
                    }
                },
                History = new List<Tuple<DateTime, string>>()
                {
                    Tuple.Create(DateTime.UtcNow, "Completed")
                }
            };
            
            await _fixture.PublishOrderCompletedEvent(orderCompletedEvent);

            // assert
            var client = new RenewalsService.RenewalsServiceClient(_renewalsGrpcChannel);
            var renewal = await client.GetRenewalDetailsForExpiringPolicyAsync(new GetRenewalDetailsForExpiringPolicyRequest { ExpiringPolicyId = expiringPolicyId });

            renewal.RenewedPolicyId.Should().Be(renewedPolicyId.ToString());
        }

        private object GetPolicyBoundEventForExpiringPolicy(string expiringPolicyId, DateTime policyBoundDate)
        {
            var offerId = Guid.Parse(expiringPolicyId);
            return new
            {
                PolicyId = expiringPolicyId,
                PolicyNumber = "ABC-123",
                Status = "Active",
                OrderId = Guid.NewGuid().ToString(),
                QuotationId = Guid.NewGuid().ToString(),
                ContactId = Guid.NewGuid().ToString(),
                ProductCode = "PI-PL-DUAL",
                ProductTypeCode = "PI-PL",
                PolicyBoundDate = policyBoundDate,
                InceptionDate = policyBoundDate,
                ExpiryDate = policyBoundDate.AddYears(1),
                OfferId = offerId
            };
        }
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
