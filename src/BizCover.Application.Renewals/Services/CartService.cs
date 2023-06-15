using BizCover.Application.Carts;

namespace BizCover.Application.Renewals.Services;

internal class CartService : ICartService
{
    private readonly CartsService.CartsServiceClient _cartsClient;

    public CartService(CartsService.CartsServiceClient cartsClient) => 
        _cartsClient = cartsClient;

    public async Task<CartDto> AddToCart(string offerId, string quotationId)
    {
        var addProductToCartRequest = new AddProductToCartRequest
        {
            CartId = Guid.NewGuid().ToString(),
            OfferId = offerId,
            QuotationId = quotationId
        };

        return (await _cartsClient.AddProductToCartAsync(addProductToCartRequest)).Cart;
    }
}
