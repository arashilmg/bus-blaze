using BizCover.Application.Offers;
using BizCover.Application.Orders;
using BizCover.Application.Quotations;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases.GenerateRenewalOrder;
using BizCover.Messages.Renewals;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BizCover.Consumer.Renewals
{
    public class GenerateAutoRenewalOrderCommandConsumer : IConsumer<GenerateAutoRenewalOrderCommand>
    {
        private readonly GenerateRenewalOrder _generateRenewalOrder;
        private readonly ILogger<GenerateAutoRenewalOrderCommandConsumer> _logger;
        private readonly IRenewalService _renewalService;
        private readonly IQueuePublisher _queuePublisher;

        public GenerateAutoRenewalOrderCommandConsumer(GenerateRenewalOrder generateRenewalOrder, 
            ILogger<GenerateAutoRenewalOrderCommandConsumer> logger, 
            IRenewalService renewalService,
            IQueuePublisher queuePublisher)
        {
            _generateRenewalOrder = generateRenewalOrder;
            _logger = logger;
            _renewalService = renewalService;
            _queuePublisher = queuePublisher;
        }

        public async Task Consume(ConsumeContext<GenerateAutoRenewalOrderCommand> context)
        {
            var response = await _generateRenewalOrder.Generate(context.Message.ExpiringPolicyId, context.CancellationToken);

            if (!response.Success)
            {
                _logger.LogInformation($"Unable to generate auto renewal order because {response.Result.FailedReason}");
                return;
            }

            await _renewalService.SetOrderGeneratedDetails(context.Message.ExpiringPolicyId, response.Result.OrderId,
                context.CancellationToken);

            // TODO : This  message is published for attaching offer and setting premium details to SF case for renewals. This needs to be integrated to the same New Business integration point.
            await _queuePublisher.Publish(new RenewalOrderGeneratedEvent
            {
                ExpiringPolicyId = context.Message.ExpiringPolicyId,
                ExpiringPolicyNumber = response.Result.PolicyNumber,
                ExpiringPolicyExpiryDate = response.Result.PolicyExpiryDate,
                OrderId = response.Result.OrderId,
                TotalAmount = response.Result.TotalAmount,
                TotalPremium =  response.Result.TotalPremium,
                ContactId = new Guid(response.Result.ContactId),
                RenewalType = "Auto",
                Offers = MapOffers(response.Result.Offers)
            }, context.CancellationToken);
        }

        private static IEnumerable<Offer> MapOffers(IEnumerable<OfferDto> offerDtos)
        {
            return offerDtos.Select(offer => new Offer
            {
                OfferId = new Guid(offer.OfferId),
                ProductTypes = MapProductTypes(offer.ProductTypes),
                Code = offer.Code,
                Expired = offer.Expired,
                ExpiryDate = offer.ExpiryDate.ToDateTime(),
                Sold = offer.Sold
            });
        }

        private static IEnumerable<ProductType> MapProductTypes(IEnumerable<OfferProductTypeDto> offerProductTypes)
        {
            return offerProductTypes.Select(productType => new ProductType
            {
                ProductTypeCode = productType.ProductTypeCode, 
                Parameters = productType.Parameters.ToDictionary(parameter => parameter.Key, parameter => parameter.Value),
            }).ToList();
        }
    }
}
