using BizCover.Application.Renewals.UseCases;
using BizCover.Messages.Policies;
using MassTransit;

namespace BizCover.Consumer.Renewals
{
    public class PolicyStatusChangedEventConsumer : IConsumer<PolicyStatusChangedEvent>
    {
        private readonly UpdatePolicyStatus _updatePolicyStatus;

        public PolicyStatusChangedEventConsumer(UpdatePolicyStatus updatePolicyStatus)
        {
            _updatePolicyStatus = updatePolicyStatus;
        }

        public async Task Consume(ConsumeContext<PolicyStatusChangedEvent> context) =>
            await _updatePolicyStatus.UpdateStatus(context.Message.PolicyId, context.Message.Status,
                context.CancellationToken);
    }
}
