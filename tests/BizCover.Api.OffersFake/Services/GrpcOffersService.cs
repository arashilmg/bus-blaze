using BizCover.Application.Offers;
using Grpc.Core;

namespace BizCover.Api.OffersFake.Services
{
    public class GrpcOffersService : OffersService.OffersServiceBase
    {



        public override Task<GetOfferResponse> GetOffer(GetOfferRequest request, ServerCallContext context)
        {
            if (request.OfferId == "00000000-0000-0000-0000-000000000001"
                || request.OfferId == "00000000-0000-0000-0000-000000000003"
                )
            {
                return Task.FromResult(new GetOfferResponse
                {
                    Offer = new OfferDto()
                    {
                        ExpiringPolicyId = string.Empty,
                        OfferType = OfferType.New
                    }
                });
            }

            if (request.OfferId == "00000000-0000-0000-0000-000000000002")
            {
                return Task.FromResult(new GetOfferResponse
                {
                    Offer = new OfferDto
                    {
                        ExpiringPolicyId = "00000000-0000-0000-0000-000000000001",
                        OfferType = OfferType.Renewal
                    }
                });
            }

            if (request.OfferId == "00000000-0000-0000-0000-000000000005")
            {
                return Task.FromResult(new GetOfferResponse
                {
                    Offer = new OfferDto
                    {
                        ExpiringPolicyId = "00000000-0000-0000-0000-000000000004",
                        OfferType = OfferType.Renewal
                    }
                });
            }

            return Task.FromResult(new GetOfferResponse
            {
                Offer = new OfferDto()
                {
                    ExpiringPolicyId = Guid.NewGuid().ToString(),
                    OfferType = OfferType.Amendment
                }
            });


        }
            
    }
}