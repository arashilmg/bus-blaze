using BizCover.Application.Policies;
using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Rules.AutoEligibility;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.gRPC.Payment;
using BizCover.Messages.Renewals;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BizCover.Consumer.Renewals.Tests.Consumer;

public class SubmitAutoRenewalOrderCommandConsumerTests
{
    private readonly Mock<ISubmitRenewalOrder> _mockSubmitRenewalOrder = new();
    private readonly Mock<ILogger<SubmitAutoRenewalOrderCommandConsumer>> _mockLogger = new();
    private readonly Mock<IRenewalService> _mockRenewalService = new();
    private readonly Mock<IOrderService> _mockOrderService = new();
    private readonly Mock<IPolicyService> _mockPolicyService = new();
    private readonly Mock<IPaymentService> _mockPaymentService = new();
    private readonly Mock<IQueuePublisher> _mockQueuePublisher = new();
    private readonly Mock<IRenewalEligibility> _mockRenewalEligibility = new();
    private readonly SubmitAutoRenewalOrderCommandConsumer _submitAutoRenewalOrderCommandConsumer;

    public SubmitAutoRenewalOrderCommandConsumerTests()
    {
        _submitAutoRenewalOrderCommandConsumer = new SubmitAutoRenewalOrderCommandConsumer(_mockLogger.Object,
                                                                                           _mockOrderService.Object,
                                                                                           _mockPolicyService.Object,
                                                                                           _mockRenewalEligibility.Object,
                                                                                           _mockPaymentService.Object,
                                                                                           _mockQueuePublisher.Object);
    }

    [Fact]
    public async Task Submit_Auto_Renewal_Order_Executed()
    {
        var orderId = Guid.NewGuid();
        var expiringPolicyId = Guid.NewGuid();
        var context = Mock.Of<ConsumeContext<SubmitAutoRenewalOrderCommand>>(_ =>
              _.Message == new SubmitAutoRenewalOrderCommand {
                  ExpiringPolicyId = expiringPolicyId,
                  OrderId = orderId,
              });

        var renewalEligibilityResponse = new RenewalEligibilityResponse()
        {
            IsEligible = true
        };

        _mockOrderService.Setup(x => x.SetAutoRenewalUserDetails(It.IsAny<Guid>()));
        _mockPolicyService.Setup(x => x.GetPolicy(It.IsAny<string>())).Returns(Task.FromResult<PolicyDto>(new PolicyDto() { PaymentFrequency = "Annual" }));
        _mockRenewalEligibility.Setup(x => x.CheckEligibility(It.IsAny<Guid>(), It.IsAny<string>(), CancellationToken.None)).Returns(Task.FromResult<RenewalEligibilityResponse>(renewalEligibilityResponse));
        _mockPaymentService.Setup(x => x.GetSubscriptionByPolicyId(It.IsAny<Guid>())).Returns(
            Task.FromResult<SubscriptionDto>(new SubscriptionDto()
            {
                CreditCard = new CreditCardDto() { PaymentProfileId = "321" }
            }));
        _mockPaymentService.Setup(x => x.CreatePendingPayment(It.IsAny<Guid>(), It.IsAny<string>()));
        
        await _submitAutoRenewalOrderCommandConsumer.Consume(context);

        _mockPaymentService.Verify(x => x.CreatePendingPayment(It.IsAny<Guid>(), It.IsAny<string>()));
        _mockOrderService.Verify(x => x.SetAutoRenewalUserDetails(It.IsAny<Guid>()), Times.Once);
    }

}
