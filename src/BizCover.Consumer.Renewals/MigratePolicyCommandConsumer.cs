using BizCover.Application.Renewals.UseCases;
using BizCover.Messages.Renewals;
using MassTransit;

namespace BizCover.Consumer.Renewals;

public class MigratePolicyCommandConsumer : IConsumer<MigratePolicyCommand>
{
    private readonly IAddOrUpdatePolicyRenewalDetails _addOrUpdatePolicyRenewalDetails;

    public MigratePolicyCommandConsumer(IAddOrUpdatePolicyRenewalDetails addOrUpdatePolicyRenewalDetails)
    {
        _addOrUpdatePolicyRenewalDetails = addOrUpdatePolicyRenewalDetails;
    }

    public async Task Consume(ConsumeContext<MigratePolicyCommand> context)
    {
        await _addOrUpdatePolicyRenewalDetails.AddOrUpdate(
            new Guid(context.Message.PolicyId),
            context.Message.ExpiryDate,
            context.Message.InceptionDate,
            context.Message.ProductCode,
            context.Message.Status,
            context.CancellationToken);
    }
}
