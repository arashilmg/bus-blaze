using BizCover.Application.Offers;
using BizCover.Application.Orders;
using BizCover.Application.Policies;
using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Services;

internal class OrderService : IOrderService
{
    private readonly Orders.OrderService.OrderServiceClient _orderClient;
    private readonly IOfferService _offerService;
    private readonly ICartService _cartService;

    public OrderService(Orders.OrderService.OrderServiceClient orderClient, 
        IOfferService offerService, 
        ICartService cartService)
    {
        _orderClient = orderClient;
        _offerService = offerService;
        _cartService = cartService;
    }

    public async Task<(OfferDto, OrderDto)> GenerateOrder(PolicyDto expiringPolicy)
    {
        // step 1: Generate offer 
        var newOffer = await _offerService.GenerateOffer(expiringPolicy, expiringPolicy.ContactId);

        // step 2: add renewal quotation to cart 
        var newCart = await _cartService.AddToCart(newOffer.OfferId, newOffer.QuotationId());

        // step 3: generate order from old order
        var newOrder = await GenerateOrder(Guid.Parse(expiringPolicy.OrderId), Guid.Parse(newCart.CartId));

        return (newOffer, newOrder);
    }

    public async Task SubmitOrder(Guid orderId) =>
        await _orderClient.SubmitOrderAsync(new SubmitOrderRequest()
        {
            OrderId = orderId.ToString()
        });

    public async Task<OrderDto> GetOrder(Guid orderId)
    {
        return (await _orderClient.GetOrderAsync(new GetOrderRequest()
        {
            OrderId = orderId.ToString()
        })).OrderDetails;
    }

    private async Task<OrderDto> GenerateOrder(Guid expiringPolicyOrderId, Guid newCartId)
    {
        // get previous order
        var expiringPolicyOrder = (await _orderClient.GetOrderAsync(new GetOrderRequest()
        {
            OrderId = expiringPolicyOrderId.ToString()
        })).OrderDetails;

        // generate order
        var newOrder = (await _orderClient.GenerateOrderAsync(new GenerateOrderRequest()
        {
            OrderId = Guid.NewGuid().ToString(),
            CartId = newCartId.ToString(),
            PaymentFrequency = expiringPolicyOrder.PaymentFrequency,
            PaymentMethod = expiringPolicyOrder.PaymentMethod,
            CardType = expiringPolicyOrder.CardType,
            LegalEntities = { expiringPolicyOrder.LegalEntities },
            LegalRepresentative = expiringPolicyOrder.LegalRepresentative
        })).OrderDetails;

        return newOrder;
    }

    public async Task SetAutoRenewalUserDetails(Guid orderId)
    {
        var input= new SetUserDetailsRequest()
        {
            OrderId = orderId.ToString(),
            SubmittedBy = new SubmittedBy
            {
                Source = SourceType.AutoRenewal,
                SubmittedUserDetails = new SubmittedUserDetails
                {
                    Email = string.Empty,
                    Id = string.Empty,
                    IdentityOrigin = IdentityType.Nil,
                    UserFullName = string.Empty
                }
            }
        };
        await _orderClient.SetUserDetailsAsync(input);
    }
}