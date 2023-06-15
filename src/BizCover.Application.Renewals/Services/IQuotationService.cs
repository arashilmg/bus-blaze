using BizCover.Application.Quotations;

namespace BizCover.Application.Renewals.Services;

public interface IQuotationService
{
    Task<QuotationDto> Get(string quotationId);
}
