using BizCover.Application.Offers;
using BizCover.Application.Orders;
using BizCover.Application.Policies;
using BizCover.Application.Quotations;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Services;

namespace BizCover.Application.Renewals.UseCases.GenerateRenewalOrder
{
    public class GenerateRenewalOrder
    {

        private readonly IOrderService _orderService;
        private readonly IRenewalEligibility _renewalEligibility;
        private readonly IRenewalService _renewalService;
        private readonly IPolicyService _policyService;
        private readonly IQuotationService _quotationService;

        public const string MonthlyPaymentFrequency = "Monthly";

        public GenerateRenewalOrder(
            IOrderService orderService,
            IRenewalEligibility renewalEligibility,
            IRenewalService renewalService,
            IPolicyService policyService,
            IQuotationService quotationService)
        {
            _orderService = orderService;
            _renewalEligibility = renewalEligibility;
            _renewalService = renewalService;
            _policyService = policyService;
            _quotationService = quotationService;
        }

        public async Task<GenerateRenewalOrderResponse> Generate(Guid expiringPolicyId, CancellationToken cancellationToken)
        {
            var expiringPolicy = await _policyService.GetPolicy(expiringPolicyId.ToString());
            var result = await _renewalEligibility.CheckEligibility(expiringPolicyId, expiringPolicy.PaymentFrequency, cancellationToken);

            if (!result.IsEligible)
            {
                return GenerateOrderFailedResponse(result.Reason);
            }

            var renewal = await _renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, cancellationToken);
            if (renewal.OrderId.HasValue)
            {
                var order = await _orderService.GetOrder(renewal.OrderId.Value);
                return GenerateOrderResponse(order, expiringPolicy);
            }

            var (newOffer, newOrder) = await _orderService.GenerateOrder(expiringPolicy);
            return await GenerateOrderResponseIncludingOffers(newOrder, expiringPolicy, newOffer);
        }

        private GenerateRenewalOrderResponse GenerateOrderResponse(OrderDto order, PolicyDto expiringPolicy)
        {
            return new GenerateRenewalOrderResponse
            {
                Success = true,
                Result = new ResultDto
                {
                    OrderId = Guid.Parse(order.OrderId),
                    ContactId = order.ContactId,
                    PolicyNumber = expiringPolicy.PolicyNumber,
                    PolicyExpiryDate = expiringPolicy.ExpiryDate.ToDateTime()
                }
            };
        }

        private async Task<GenerateRenewalOrderResponse> GenerateOrderResponseIncludingOffers(OrderDto order, PolicyDto expiringPolicy, OfferDto offer)
        {
            var quotation = (await _quotationService.Get(offer.QuotationId()));

            return new GenerateRenewalOrderResponse
            {
                Success = true,
                Result = new ResultDto
                {
                    OrderId = Guid.Parse(order.OrderId),
                    TotalAmount = GetPolicyTotalAmount(quotation, order),
                    TotalPremium = quotation.Product.TotalPremium.DecimalValue,
                    ContactId = order.ContactId,
                    PolicyNumber = expiringPolicy.PolicyNumber,
                    PolicyExpiryDate = expiringPolicy.ExpiryDate.ToDateTime(),
                    Offers = new List<OfferDto> { offer },
                }
            };
        }

        private static decimal GetPolicyTotalAmount(QuotationDto quotation, OrderDto order)
        {
            return
                order.PaymentFrequency == MonthlyPaymentFrequency ?
                    quotation.Product.TotalMonthlyPremium.DecimalValue + order.TotalTransactionCharge.DecimalValue :
                    quotation.Product.TotalPremium.DecimalValue + order.TotalTransactionCharge.DecimalValue;
        }

        private static GenerateRenewalOrderResponse GenerateOrderFailedResponse(string message)
        {
            return new GenerateRenewalOrderResponse
            {
                Success = false,
                Result = new ResultDto
                {
                    FailedReason = message
                }
            };
        }
    }
}
