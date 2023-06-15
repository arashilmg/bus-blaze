using BizCover.Application.Offers;
using BizCover.Application.Orders;
using BizCover.Application.Policies;

namespace BizCover.Application.Renewals.Services;

public interface IOrderService
{
    Task<(OfferDto offer, OrderDto order)> GenerateOrder(PolicyDto expiringPolicy);
    Task SubmitOrder(Guid orderId);
    Task<OrderDto> GetOrder(Guid orderId);

    Task SetAutoRenewalUserDetails(Guid orderId);
}
