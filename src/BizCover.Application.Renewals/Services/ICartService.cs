using BizCover.Application.Carts;

namespace BizCover.Application.Renewals.Services;

public interface ICartService
{
    Task<CartDto> AddToCart(string offerId, string quotationId);
}