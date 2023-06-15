using BizCover.Application.Offers;
namespace BizCover.Application.Renewals.Helpers;

internal static class OfferDtoExtension
{
    internal static string QuotationId(this OfferDto offer) =>
        offer.ProductTypes.First().Quotations.First().QuotationId;

    internal static string InceptionDate(this OfferDto offer) =>
        offer.ProductTypes.First().Parameters[ParameterName.InceptionDate];

    internal static string ExpirationDate(this OfferDto offer) =>
        offer.ProductTypes.First().Parameters[ParameterName.ExpirationDate];
}
