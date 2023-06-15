using BizCover.Application.Quotations;

namespace BizCover.Application.Renewals.Services;

internal class QuotationService : IQuotationService
{
    private readonly QuotationsService.QuotationsServiceClient _quoteClient;

    public QuotationService(QuotationsService.QuotationsServiceClient quoteClient)
    {
        _quoteClient = quoteClient;
    }

    public async Task<QuotationDto> Get(string quotationId) => 
        (await _quoteClient.GetQuotationAsync(new GetQuotationRequest{QuotationId = quotationId})).Quotation;
}