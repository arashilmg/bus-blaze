using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases;
using BizCover.Entity.Renewals;
using BizCover.Messages.Policies;
using BizCover.Messages.Renewals;
using MassTransit;

namespace BizCover.Consumer.Renewals;

public class PolicyBoundEventConsumer : IConsumer<PolicyBoundEvent>
{
    private readonly IAddOrUpdatePolicyRenewalDetails _addOrUpdatePolicyRenewalDetails;
    private readonly IRenewalService _renewalService;
    private readonly IQueuePublisher _queuePublisher;
    
    public PolicyBoundEventConsumer(IAddOrUpdatePolicyRenewalDetails addOrUpdatePolicyRenewalDetails, 
        IRenewalService renewalService, 
        IQueuePublisher queuePublisher)
    {
        _addOrUpdatePolicyRenewalDetails = addOrUpdatePolicyRenewalDetails;
        _renewalService = renewalService;
        _queuePublisher = queuePublisher;
    }

    public async Task Consume(ConsumeContext<PolicyBoundEvent> context)
    {
        await _addOrUpdatePolicyRenewalDetails.AddOrUpdate(
            context.Message.PolicyId,
            context.Message.ExpiryDate,
            context.Message.InceptionDate,
            context.Message.ProductCode,
            context.Message.Status,
            context.CancellationToken);

        var renewal = await _renewalService.GetRenewalDetailsForRenewedPolicy(context.Message.PolicyId, context.CancellationToken);

        if (renewal != null)
        {
            await PublishPolicyRenewedEvent(renewal, context.CancellationToken);
        }
    }


    public async Task PublishPolicyRenewedEvent(Renewal renewal, CancellationToken cancellationToken)
    {
        var policyRenewedEvent = new PolicyRenewedEvent
        {
            ExpiringPolicyId = renewal.ExpiringPolicyId,
            RenewedPolicyId = renewal.RenewedPolicyId!.Value,
            RenewalDate = renewal.RenewedDate!.Value
        };
        await _queuePublisher.Publish(policyRenewedEvent, cancellationToken);
    }
}
