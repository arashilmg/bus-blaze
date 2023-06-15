using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BizCover.Application.Offers;
using BizCover.Application.Orders;
using BizCover.Application.Policies;
using BizCover.Application.Renewals.Services;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using DateTime = System.DateTime;

namespace BizCover.Api.Renewals.IntegrationTests.SubmitAutoRenewalOrder
{
    public class OrderServiceStub : IOrderService
    {
        public Task<(OfferDto offer, OrderDto order)> GenerateOrder(PolicyDto expiringPolicy)
        {
            return Task.FromResult((
                new OfferDto()
                {
                    OfferId = Guid.NewGuid().ToString(),
                    Code = 123,
                    Expired = false,
                    ExpiryDate = DateTime.UtcNow.AddDays(30).ToTimestamp(),
                    Sold = false,
                    ProductTypes =
                    {
                        new OfferProductTypeDto()
                        {
                            ProductTypeCode = "PI-PL",
                            Parameters = { new Dictionary<string, string>() },
                            Quotations = { new OfferQuotationDto()
                            {
                                QuotationId = Guid.NewGuid().ToString()
                            } }
                        }
                    }
                }
                , new OrderDto()
                {
                    OrderId = Guid.NewGuid().ToString(),
                    TotalTransactionCharge = new Money() { DecimalValue = 100 },
                    ContactId = Guid.NewGuid().ToString(),
                    PaymentFrequency = "Monthly"
                }));
        }

        public Task SubmitOrder(Guid orderId)
        {
            return Task.CompletedTask;
        }

        public Task<OrderDto> GetOrder(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task SetAutoRenewalUserDetails(Guid orderId)
        {
            return Task.CompletedTask;
        }
    }
}
