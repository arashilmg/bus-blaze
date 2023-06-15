using BizCover.Application.Offers;
using BizCover.Application.Policies;
using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Services;

internal class OfferService : IOfferService
{
    private readonly OffersService.OffersServiceClient _offersClient;

    public OfferService(OffersService.OffersServiceClient offersClient) =>
        _offersClient = offersClient;

    public async Task<OfferDto> GenerateOffer(PolicyDto expiringPolicy, string contactId)
    {
        var offerRequest = CreateRequest(expiringPolicy, contactId);

        var offer = (await _offersClient.GenerateOfferForRenewalAsync(offerRequest)).Offer;

        return offer;
    }

    private static GenerateOfferForRenewalRequest CreateRequest(PolicyDto expiringPolicy, string contactId)
    {
        return new GenerateOfferForRenewalRequest
        {
            OfferId = Guid.NewGuid().ToString(),
            ProductCode = expiringPolicy.ProductCode,
            Parameters = { PrepareParameters(expiringPolicy) },
            PreviousQuotationId = expiringPolicy.QuotationId,
            ExpiringPolicyId = expiringPolicy.PolicyId,
            ContactId = contactId
        };
    }

    private static Dictionary<string, string> PrepareParameters(PolicyDto expiringPolicy)
    {
        var parameters = expiringPolicy.Parameters.ToDictionary(x => x.Code, y => y.Value);

        parameters[ParameterName.InceptionDate] = DateHelper.CalculateInceptionDate(expiringPolicy.ExpiryDate.ToDateTime());
        parameters[ParameterName.ExpirationDate] = DateHelper.CalculateExpiryDates(expiringPolicy.ExpiryDate.ToDateTime());

        parameters[ParameterName.ProfessionalIndemnityCurrentCoverInsurerCode] = ParameterValue.BizCover;
        parameters[ParameterName.ProfessionalIndemnityCurrentCoverInForce] = ParameterValue.True;

        parameters[ParameterName.IndustryCode] = parameters.GetSection(ParameterName.OccupationSelection, 0);
        parameters[ParameterName.OccupationCode] = parameters.GetSection(ParameterName.OccupationSelection, 1);

        parameters[ParameterName.CountryCode] = parameters.GetSection(ParameterName.LocationSelection, 0);
        parameters[ParameterName.StateCode] = parameters.GetSection(ParameterName.LocationSelection, 1);
        parameters[ParameterName.PostCode] = parameters.GetSection(ParameterName.LocationSelection, 2);
        parameters[ParameterName.Locality] = parameters.GetSection(ParameterName.LocationSelection, 3);
        parameters[ParameterName.Street] = parameters.GetSection(ParameterName.LocationSelection, 4); ;

        return parameters;
    }

    public async Task<OfferDto> GetOffer(string offerId)
        => (await _offersClient.GetOfferAsync(new GetOfferRequest { OfferId = offerId })).Offer;
}
