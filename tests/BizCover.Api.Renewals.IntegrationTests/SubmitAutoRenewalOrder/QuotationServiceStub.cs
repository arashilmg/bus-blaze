using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizCover.Application.Quotations;
using BizCover.Application.Renewals.Services;
using Google.Type;

namespace BizCover.Api.Renewals.IntegrationTests.SubmitAutoRenewalOrder
{
    public class QuotationServiceStub : IQuotationService
    {
        public Task<QuotationDto> Get(string quotationId)
        {
            var sections = new List<SectionQuotationDto>
            {
                new()
                {
                    LineItems =
                    {
                        new LineItemDto
                        {
                            LineItemCode = LineItemCodes.BasePremium,
                            Amount = new Money { DecimalValue = 1, CurrencyCode = "AUD" }
                        },
                        new LineItemDto
                        {
                            LineItemCode = LineItemCodes.BasePremiumGst,
                            Amount = new Money { DecimalValue = 2, CurrencyCode = "AUD" }
                        },
                        new LineItemDto
                        {
                            LineItemCode = LineItemCodes.BasePremiumGstStampDuty,
                            Amount = new Money { DecimalValue = 3, CurrencyCode = "AUD" }
                        }
                    }
                },
                new()
                {
                    LineItems =
                    {
                        new LineItemDto
                        {
                            LineItemCode = LineItemCodes.BasePremium,
                            Amount = new Money { DecimalValue = 4, CurrencyCode = "AUD" }
                        },
                        new LineItemDto
                        {
                            LineItemCode = LineItemCodes.BasePremiumGst,
                            Amount = new Money { DecimalValue = 5, CurrencyCode = "AUD" }
                        },
                        new LineItemDto
                        {
                            LineItemCode = LineItemCodes.BasePremiumGstStampDuty,
                            Amount = new Money { DecimalValue = 6, CurrencyCode = "AUD" }
                        }
                    }
                }
            };

            var lineitems = new List<LineItemDto>
            {
                new()
                {
                    LineItemCode = LineItemCodes.UnderwritingFee,
                    Amount = new Money { DecimalValue = 1, CurrencyCode = "AUD" }
                },
                new()
                {
                    LineItemCode = LineItemCodes.UnderwritingFeeGst,
                    Amount = new Money { DecimalValue = 2, CurrencyCode = "AUD" }
                },
                new()
                {
                    LineItemCode = LineItemCodes.PlatformFee,
                    Amount = new Money { DecimalValue = 3, CurrencyCode = "AUD" }
                },
                new()
                {
                    LineItemCode = LineItemCodes.PlatformFeeGst,
                    Amount = new Money { DecimalValue = 4, CurrencyCode = "AUD" }
                }
            };

            var quotation = new QuotationDto
            {
                Product = new ProductQuotationDto
                {
                    TotalPremium = new Money { DecimalValue = 31, CurrencyCode = "AUD" },
                    TotalMonthlyPremium = new Money { DecimalValue = 3, CurrencyCode = "AUD"},
                    Sections = { sections },
                    LineItems = { lineitems }
                }
            };

            return Task.FromResult(quotation);

        }
    }

    public class LineItemCodes
    {
        public const string UnderwritingFee = nameof(UnderwritingFee);
        public const string UnderwritingFeeGst = nameof(UnderwritingFeeGst);

        public const string PlatformFee = nameof(PlatformFee);
        public const string PlatformFeeGst = nameof(PlatformFeeGst);

        public const string BasePremium = nameof(BasePremium);
        public const string BasePremiumGst = nameof(BasePremiumGst);
        public const string BasePremiumGstStampDuty = nameof(BasePremiumGstStampDuty);

        public const string InstalmentFee = nameof(InstalmentFee);
        public const string InstalmentFeeGst = nameof(InstalmentFeeGst);

        public const string CreditCardFee = nameof(CreditCardFee);
        public const string CreditCardFeeGst = nameof(CreditCardFeeGst);

    }
}
