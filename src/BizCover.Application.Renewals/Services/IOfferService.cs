using BizCover.Application.Offers;
using BizCover.Application.Policies;

namespace BizCover.Application.Renewals.Services;

public interface IOfferService
{
    Task<OfferDto> GenerateOffer(PolicyDto expiringPolicy, string contactId);
    Task<OfferDto> GetOffer(string offerId);
}