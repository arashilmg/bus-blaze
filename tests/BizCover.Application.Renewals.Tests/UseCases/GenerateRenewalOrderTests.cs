using BizCover.Application.Offers;
using BizCover.Application.Orders;
using BizCover.Application.Policies;
using BizCover.Application.Quotations;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases.GenerateRenewalOrder;
using BizCover.Entity.Renewals;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Moq;
using Xunit;
using LineItemDto = BizCover.Application.Orders.LineItemDto;

namespace BizCover.Application.Renewals.Tests.UseCases
{
    public class GenerateRenewalOrderTests
    {
        [Fact]
        public async Task Generate_RenewalOrder_If_Eligible_And_Order_Is_Not_Already_Generated()
        {
            // Arrange
            var policyService = new Mock<IPolicyService>();
            var orderService = new Mock<IOrderService>();
            var renewalService = new Mock<IRenewalService>();
            var quotationService = new Mock<IQuotationService>();
            var renewalEligibility = new Mock<IRenewalEligibility>();

            var generateRenewalOrder = new GenerateRenewalOrder(orderService.Object, renewalEligibility.Object, renewalService.Object, policyService.Object, quotationService.Object);

            renewalEligibility.Setup(x => x.CheckEligibility(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new RenewalEligibilityResponse()
                {
                    IsEligible = true
                }));

            renewalService.Setup(x => x.GetRenewalDetailsForExpiringPolicy(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(new Renewal()
                {
                    OrderId = null
                }));

            policyService.Setup(x => x.GetPolicy(It.IsAny<string>())).Returns(() => Task.FromResult(new PolicyDto()
            {
                OrderId = "C72436C5-9322-4C50-88BE-251BD4699BDC",
                InceptionDate = new DateTime(2021, 01, 01).ToUniversalTime().ToTimestamp(),
                ExpiryDate = new DateTime(2022, 01, 01).ToUniversalTime().ToTimestamp()
            }));
            var offerDto = new OfferDto();
            var offerProductTypeDto = new OfferProductTypeDto();
            var offerQuotationDto = new OfferQuotationDto {
                    QuotationId = "d1a4d4f0-6e80-4777-9247-d08734ebc77d",
                    ProductCode = "PI-PL-VERO",
                    TotalPremium = new() { DecimalValue = 638 },
                    TotalMonthlyPremium = new() { DecimalValue = 53 }
            };

            offerProductTypeDto.Quotations.Add(offerQuotationDto);
            offerDto.ProductTypes.Add(offerProductTypeDto);

            orderService.Setup(x => x.GenerateOrder(It.IsAny<PolicyDto>())).Returns(() =>
                Task.FromResult((
                    offerDto,
                    new OrderDto()
                    {
                        OrderId = Guid.NewGuid().ToString(),
                        LineItems = 
                        { 
                            new RepeatedField<LineItemDto>
                            {
                                new LineItemDto
                                {
                                    QuotationId = Guid.NewGuid().ToString()
                                }
                            }
                        },
                        TotalLineItemAmount = new() { DecimalValue = 100 },
                        TotalTransactionCharge = new() { DecimalValue = 100 }
                    })));

            quotationService.Setup(x => x.Get(It.IsAny<string>())).Returns(() => Task.FromResult(new QuotationDto()
            {
                Product = new ProductQuotationDto()
                {
                    TotalPremium = new Google.Type.Money { CurrencyCode = "AUD", DecimalValue = 200 },
                    TotalMonthlyPremium = new Google.Type.Money { CurrencyCode = "AUD", DecimalValue = 200 },
                     
                }
            }));

            // Act
            var response = await generateRenewalOrder.Generate(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            orderService.Verify(x => x.GenerateOrder(It.IsAny<PolicyDto>()), Times.Once);
        }

        [Fact]
        public async Task Return_Already_Generated_Order_If_Order_Is_Already_Generated()
        {
            // Arrange
            var policyService = new Mock<IPolicyService>();
            var orderService = new Mock<IOrderService>();
            var renewalService = new Mock<IRenewalService>();
            var renewalEligibility = new Mock<IRenewalEligibility>();
            var quotationService = new Mock<IQuotationService>();

            var generateRenewalOrder = new GenerateRenewalOrder(orderService.Object, renewalEligibility.Object, renewalService.Object, policyService.Object, quotationService.Object);

            renewalEligibility.Setup(x => x.CheckEligibility(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new RenewalEligibilityResponse()
                {
                    IsEligible = true
                }));

            var orderId = new Guid("A72436C5-9322-4C50-88BE-251BD4699BDA");
            renewalService.Setup(x => x.GetRenewalDetailsForExpiringPolicy(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(() => Task.FromResult(new Renewal()
                {
                    OrderId = orderId
                }));

            orderService.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                .Returns(() => Task.FromResult(new OrderDto()
                {
                    OrderId = orderId.ToString(),
                    LineItems =
                    {
                        new RepeatedField<LineItemDto>
                        {
                            new LineItemDto
                            {
                                QuotationId = Guid.NewGuid().ToString()
                            }
                        }
                    },
                    TotalLineItemAmount = new() { DecimalValue = 100 },
                    TotalTransactionCharge = new() { DecimalValue = 100 }
                }));

            policyService.Setup(x => x.GetPolicy(It.IsAny<string>())).Returns(() => Task.FromResult(new PolicyDto()
            {
                OrderId = "C72436C5-9322-4C50-88BE-251BD4699BDC",
                InceptionDate = new DateTime(2021, 01, 01).ToUniversalTime().ToTimestamp(),
                ExpiryDate = new DateTime(2022, 01, 01).ToUniversalTime().ToTimestamp()
            }));

            quotationService.Setup(x => x.Get(It.IsAny<string>())).Returns(() => Task.FromResult(new QuotationDto()
            {
                Product = new ProductQuotationDto()
                {
                    TotalPremium = new Google.Type.Money { CurrencyCode = "AUD", DecimalValue = 200 },
                    TotalMonthlyPremium = new Google.Type.Money { CurrencyCode = "AUD", DecimalValue = 200 },
                }
            }));

            // Act
            var response = await generateRenewalOrder.Generate(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(orderId, response.Result.OrderId);
            orderService.Verify(x => x.GenerateOrder(It.IsAny<PolicyDto>()), Times.Never);
        }

        [Fact]
        public async Task DoNot_Generate_Order_If_Not_Eligible()
        {
            // Arrange
            var policyService = new Mock<IPolicyService>();
            var orderService = new Mock<IOrderService>();
            var renewalService = new Mock<IRenewalService>();
            var quotationService = new Mock<IQuotationService>();
            var renewalEligibility = new Mock<IRenewalEligibility>();

            var generateRenewalOrder = new GenerateRenewalOrder(orderService.Object, renewalEligibility.Object, renewalService.Object, policyService.Object, quotationService.Object);

            renewalEligibility.Setup(x => x.CheckEligibility(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new RenewalEligibilityResponse()
                {
                    IsEligible = false
                }));

            policyService.Setup(x => x.GetPolicy(It.IsAny<string>())).Returns(() => Task.FromResult(new PolicyDto()
            {
                OrderId = "C72436C5-9322-4C50-88BE-251BD4699BDC",
                InceptionDate = new DateTime(2021, 01, 01).ToUniversalTime().ToTimestamp(),
                ExpiryDate = new DateTime(2022, 01, 01).ToUniversalTime().ToTimestamp(),
                PaymentFrequency = Constant.PaymentFrequencyMonthly
            }));

            // Act
            var response = await generateRenewalOrder.Generate(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            orderService.Verify(x => x.GenerateOrder(It.IsAny<PolicyDto>()), Times.Never);
        }
    }
}